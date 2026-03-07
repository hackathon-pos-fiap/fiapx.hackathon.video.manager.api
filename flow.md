# 📘 Fluxo de Processamento de Vídeo - Teste de Mesa

Este documento detalha o ciclo de vida de uma requisição de processamento de vídeo, desde o upload até o download do arquivo processado, utilizando a arquitetura de **Pre-signed URLs** e **Processamento Assíncrono Orientado a Eventos (Event-Driven)**.

## 🎭 Atores Envolvidos

* **Cliente:** Aplicação Frontend ou Postman (Usuário).
* **Video Manager API:** Microsserviço de gestão (.NET 8).
* **AWS S3:** Armazenamento de objetos (Vídeos Raw e ZIPs processados).
* **AWS SQS:** Fila de mensageria para desacoplamento.
* **Video Worker:** Serviço de processamento em background (.NET 8) com FFmpeg.
* **AWS SES:** Serviço de envio de e-mails transacionais (Notificações de erro).

---

## 🔄 Passo a Passo do Fluxo

### 1. Intenção de Upload (Solicitação de Permissão)
O cliente solicita permissão para enviar um arquivo, sem trafegar o binário pela nossa API, poupando recursos do servidor.

* **Ação do Cliente:** `POST /api/videos`
    * *Body:* `{ "fileName": "ferias.mp4" }`
    * *Auth:* Bearer Token (JWT Cognito)
* **Processamento da API:**
    1. Valida o JWT do usuário.
    2. Gera um ID único (`VID-123`) e salva no MongoDB com status `WAITING_UPLOAD`.
    3. Solicita à AWS SDK uma **Pre-signed URL** (PUT) válida por 10 minutos.
* **Resposta (200 OK):**
    ```json
    {
      "id": "VID-123",
      "uploadUrl": "https://meu-bucket.s3.amazonaws.com/raw/VID-123.mp4?AWSAccessKeyId=..."
    }
    ```

### 2. Upload Direto (Banda Larga)
O cliente transfere o arquivo diretamente para a nuvem da AWS.

* **Ação do Cliente:** `PUT {uploadUrl}`
    * *Body:* Arquivo binário (`video/mp4`).
* **Processamento AWS S3:**
    1. Valida a assinatura criptográfica da URL.
    2. Recebe e armazena o arquivo de forma segura.
* **Resposta:** `200 OK` (Retornada diretamente pela AWS).

### 3. Gatilho Automático (S3 Event Notification)
Nesta etapa, o cliente já está liberado. A própria infraestrutura avisa o sistema que há trabalho a ser feito.

* **Ação do Cliente:** Nenhuma. (Processo 100% transparente).
* **Processamento AWS S3 & SQS:**
    1. O S3 detecta a criação de um novo objeto (`s3:ObjectCreated:Put`) com a extensão `.mp4`.
    2. O S3 publica automaticamente um evento JSON na fila **AWS SQS**, contendo os metadados do arquivo (ex: `raw/VID-123.mp4`).

### 4. Processamento Assíncrono (Worker)
O serviço em background consome a fila e executa o trabalho pesado de extração de frames.

* **Ação do Worker:**
    1. **Consumo:** Lê a mensagem de evento da fila SQS.
    2. **Identificação:** Extrai a chave do arquivo e atualiza o MongoDB para `PROCESSING`.
    3. **Download:** Baixa o vídeo do S3 (`raw/VID-123.mp4`) para um disco local temporário.
    4. **Processamento (Snapshot):** Executa o `FFmpeg` extraindo frames do vídeo (Regra: 1 frame a cada 10s).
    5. **Finalização e Sucesso:**
        * Gera um arquivo `.zip` com as imagens.
        * Faz o upload do ZIP para o S3 (`processed/VID-123.zip`).
        * Atualiza o MongoDB para status `FINISHED`.
    6. **Tratamento de Erros e Notificação:**
        * Em caso de falha (arquivo corrompido, formato inválido, erro no FFmpeg), o Worker atualiza o status no banco para `ERROR`.
        * Aciona o **AWS SES** para enviar um e-mail transacional ao usuário, informando detalhadamente sobre a falha no processamento.
    7. **Limpeza:** Remove a mensagem da SQS e apaga os arquivos temporários do disco do container.

### 5. Consulta e Download
O usuário verifica o status e baixa o resultado.

* **Ação do Cliente:** `GET /api/videos`
* **Processamento da API:**
    1. Consulta os registros do usuário no MongoDB.
    2. Se o status for `FINISHED`, gera uma nova **Pre-signed URL** (GET) para leitura do arquivo ZIP.
* **Resposta (200 OK):**
    ```json
    [
      {
        "id": "VID-123",
        "status": "FINISHED",
        "downloadUrl": "https://meu-bucket.s3.amazonaws.com/processed/VID-123.zip?Signature=..."
      }
    ]
    ```