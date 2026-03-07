# 🛡️ Regras de Negócio e Decisões Arquiteturais

Este documento define as fronteiras operacionais do MVP do Sistema de Processamento de Vídeos e justifica as escolhas técnicas adotadas para o Hackathon FIAP X.

## 1. Regras de Negócio (Limitações do Sistema)

Para garantir a performance, segurança e a viabilidade do MVP dentro da infraestrutura proposta, as seguintes restrições foram aplicadas e validadas pela arquitetura:

| Regra | Definição | Justificativa |
| :--- | :--- | :--- |
| **Tamanho Máximo** | **20 MB** por vídeo. | Evitar custos excessivos de transferência de rede e *timeouts* de processamento no ambiente de container do Worker. |
| **Duração do Vídeo** | Mínimo **10s**, Máximo **2 min**. | Vídeos muito curtos não geram frames suficientes para um resumo útil; vídeos muito longos travam a fila e ocupam o Worker por tempo excessivo. |
| **Formato de Arquivo** | Apenas **.MP4**. | Padronização rigorosa dos codecs de entrada para evitar erros de compatibilidade e falhas de decodificação no FFmpeg. |
| **Taxa de Frames** | **1 frame a cada 10 segundos**. | Evita a geração de milhares de imagens inúteis em vídeos comuns (30fps), focando apenas em *snapshots* significativos que mostrem a progressão do vídeo. |
| **Retenção de Dados** | Arquivos (Raw e Zip) expiram em **24 horas**. | Política de *Lifecycle* do S3 garante limpeza automática, evitando acúmulo de lixo digital e custos desnecessários de armazenamento. |
| **Validade da URL (TTL)** | URLs de Upload/Download expiram em **10 min**. | **Segurança (Least Privilege):** Evita que links vazados sejam usados posteriormente para acessar ou injetar arquivos no storage da aplicação. |
| **Simultaneidade** | Usuários podem enviar múltiplos vídeos, mas o processamento é **FIFO**. | O uso de filas (First-In, First-Out) atua como *buffer*. Garante que o sistema não caia sob picos de carga, convertendo estresse de infraestrutura em apenas tempo de espera. |

---

## 2. Decisões Arquiteturais (ADR - Architecture Decision Records)



### 2.1. Orquestração de Upload: Fluxo Orientado a Eventos (S3 Trigger)
**Decisão:** O fluxo de processamento é iniciado automaticamente. O cliente realiza o upload para o S3 (via Pre-signed URL) e o próprio bucket dispara uma notificação para a fila SQS (`s3:ObjectCreated:Put`), sem necessidade de chamada manual à API.
* **Justificativa - Resiliência:** Elimina o risco de "arquivos órfãos" caso a conexão do usuário caia logo após o upload, mas antes de confirmar. Se o arquivo chegou no S3, ele *será* processado.
  * **Desacoplamento:** Adota o padrão Cloud Native, onde a infraestrutura reage a eventos, reduzindo a carga HTTP no nosso API Gateway.

### 2.2. Uso de Pre-signed URLs ("Claim Check Pattern")
**Decisão:** Utilização de URLs assinadas criptograficamente e temporárias para delegação de Upload e Download.
* **Justificativa:** Retira a pesada carga de tráfego de dados (I/O) do microsserviço da API. O cluster gerencia apenas metadados leves (JSON), enquanto a transferência massiva de arquivos de vídeo é delegada para a infraestrutura global e otimizada do AWS S3, permitindo escalabilidade horizontal sem gargalos de rede na aplicação.



### 2.3. Banco de Dados: Abordagem Híbrida (Polyglot Persistence)
**Decisão:** Manter **PostgreSQL** para o domínio de Identidade/Autenticação e adotar **MongoDB** para o domínio de Dados de Vídeo.
* **Justificativa:**
  * **PostgreSQL (Auth):** Garante integridade referencial, modelagem relacional rigorosa e consistência estrita (ACID) para dados sensíveis de usuários e credenciais.
  * **MongoDB (Vídeo):** Oferece flexibilidade de schema (*schemaless*) ideal para armazenar metadados de mídia que podem evoluir, além de altíssima performance de escrita/leitura para os logs rápidos de transição de status de processamento do Worker.

### 2.4. Mensageria: AWS SQS
**Decisão:** Uso de fila totalmente gerenciada (Amazon SQS) para enfileiramento das tarefas de vídeo.
* **Justificativa:** Reduz a carga operacional do time em comparação a manter um cluster de mensageria próprio (como RabbitMQ/Kafka) dentro do Kubernetes. O SQS é *Serverless*, oferece alta durabilidade de mensagens e possui integração nativa, segura e imediata com os eventos do bucket S3.