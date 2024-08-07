
# TechChallenge-LanchoneteTotem

Sistema de solicitação de pedido via totem de autoatendimento para lanchonete criado em .Net 7 utilizando Minimal Api e mongoDB, seguindo o padrão de arquitetura hexagonal. Proposta realizada como meio avaliativo da Fiap Pos Tech 


 ## Proposta atual

Em cada fase do projeto é abordada uma proposta diferente, você pode encontrar as fases no final do readme, atualmente este repositório está na fase 5, exemplificada abaixo:

1. Utilizar o Padrão Saga para garantir uma melhor experiência na utilização do projeto.
   - Padrão escolhido foi a Saga Coreográfada, a justificativa completa se encontra no link da documentação abaixo junto da arquitetura do projeto + arquitetura da nuvem;
   - Dentro do fluxo de funcionalmento foi solicitado que pedidos não pagos não cheguem até a cozinha evitando que os pedidos sejam preparados sem um pagamento devido. Foi implementado para que apenas após a confirmação do pagamento o pedido seja enviado para o serviço responsável por gerenciar os pedidos prontos para serem preparados.
   - Foi solicitado a utilização de um serviço de mensageria, nesse caso foi utilizado o SQS da AWS para a comunicação entre os microsserviços;

2. Executar a ferramenta OWASP Zap nos seguintes fluxos:
   - Listar/Exibir Cardápio ( Esse fluxo permanece no repositório atual dentro do monolíto)
   - Realização pedido (checkout) (Esse fluxo pertence ao MS-Pedido)
   - Geração de Pagamento (Esse fluxo pertence ao MS-Pagamento)
   - Confirmação do Pagamento / Fila de Preparação de pedidos (Esse fluxo pertence ao MS-Producao)
   - Gerar os relatórios com vulnerabilidades e correções e disponibilizar nos readmes
  
   Obs. Foram gerados os relatórios e cada link está disponível nos respectivos repositórios dos projetos, serão disponibilizados os links para os repositórios abaixo, também serão adicionados os relatórios referentes ao fluxo de Listar/Exibição Cardápio.

3. Considerando a LGPD foi solicitado:
   - A criação de um relatório de impacto dos dados pessoais (RIPD) que terá seu link disponibilizado abaixo;
   - A criação de uma api para que o cliente possa solicitar a exclusão ou inativação de seus dados pessoais. Contendo os seguntes campos:
      - Nome
      - Endereço
      - Número de telefone
      - Informações de Pagamento (caso seja armazenado em algum local)
    

Links:
- Documentação Justificativa SAGA Coreográfada + Arquitetura Microsserviços + Arquitetura Nuvem (Basta descer até a Fase 5): https://www.figma.com/board/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?node-id=0%3A1&t=oY6vBdqPodcM5LMR-1

- Link para o video explicando fase 5: https://youtu.be/_8Dvd5Me59w

- Link para os relatórios OWASP ZAP:

    - Vulnerabilidades: https://fiap-docs.s3.amazonaws.com/OWASP+ZAP+Relatorios/Vulnerabilidades/Techchallenge-LanchoneteTotem-Cardapio.html
    - Correções: https://fiap-docs.s3.amazonaws.com/OWASP+ZAP+Relatorios/Correcoes/Techchallenge-LanchoneteTotem-Cardapio.html

- Relatório RIPD: https://fiap-docs.s3.amazonaws.com/RIPD/Relatorio-RIPD-GRUPO-45.pdf

- Repositórios relacionados:
    - MS-Pedido: https://github.com/WillianViegas/techchallenge-microservico-pedido
    - MS-Pagamento: https://github.com/WillianViegas/techchallenge-microservico-pagamento
    - MS-Producao: https://github.com/WillianViegas/techchallenge-microservico-producao
    - MS-Cancelamento-Dados:  https://github.com/WillianViegas/techchallenge-microservico-cancelamento-dados
  

## Estutura

Para um melhor entendimento da estrutura e como realizar o fluxo e utilizar o projeto segue uma breve descrição:
O projeto tem 3 modulos que se comunicam entre si:
- `Api TechChallenge-LanchoneteTotem` (Se trata da api principal, permitindo o CRUD de Produtos e Categorias, além da realização do pedido e seu controle/visualização);
  
- `Api Notifier` (Se trata de uma api que faz um envio para a fila do SQS informando que o pagamento de um pedido foi confirmado [Obs: Essa api existe por conta de ainda não ter sido implementado um terceiro para gerenciar o pagamento no processo. Então o notifier serve como  auxiliar para que possamos confirmar os nossos pedidos e fazer o fluxo rodar]);
  
- `Webhook PagamentoPedidoNotificationConsumer` (Se trata de um Webhook que fica ouvindo uma fila, a mesma em que o Notifier envia a confirmação do pedido, quando chegam mensagens na fila o webhook as captura e realiza a confirmação do pedido atualizando seu status e salvando no banco de dados)

Estrutura visual dos três modulos interagindo:
![Screenshot_2](https://github.com/WillianViegas/TechChallenge-LanchoneteTotem/assets/58482678/7449fe57-e093-4142-9e8c-4d285fa9304c)

Em relação a utilização do serviço da AWS SQS (Simple Queue Service) para facilitar os testes foi implementada a ferramenta Localstack que simula estes serviços sem custo e de forma mais prática para desenvolvimento local. Porém também foi configurado e testado o mesmo fluxo para a utilização do serviço na AWS;

### Inicializando o sistema
* A seguir estarão disponíveis instruções para a execução na sua máquina local para fins de teste e desenvolvimento.  

### Pré-Requisitos
* Possuir o docker instalado:
    https://www.docker.com/products/docker-desktop/

## Rodando ambiente com Docker

Acesse o diretório em que o repositório foi clonado através do terminal e
execute os comandos:
 - `docker-compose build` para compilar imagens, criar containers etc.
 - `docker-compose up` para criar os containers do banco de dados e do projeto

### Iniciando e finalizando containers
Para inicializar execute o comando `docker-compose start` e
para finalizar `docker-compose stop`

### Acessar swagger
Após a subida dos containers basta acessar: 

Api TechChallenge-LanchoneteTotem
https://localhost:7004/swagger/index.html

Api Notifier
https://localhost:7008/swagger/index.html

### Portas
Aplicação:
- Api TechChallenge-LanchoneteTotem

        7004:443
        7003:80

- Api Notifier

        7008:443
        7007:80

- Webhook PagamentoPedidoNotificationConsumer

        7006:443
        7005:80

MongoDb:

    27017:27017

LocalStack:

    4566:4566 

 ## Populando o banco de dados para os dados iniciais

 Existe uma requisição para popular o banco num primeiro momento, criando 2 usuários e algumas categorias e produtos para facilitar o entendimento inicial do sistema. 
 
 Você pode acessa-lá pelo swagger ou fazer a requisição você mesmo. (obs. ela só faz a inserção caso as tabelas de usuário, categoria ainda estejam vazias); 

    endpoint + /seed


## Rodando ambiente Kubernetes
* Possuir kubernetes instalados:
    https://kubernetes.io/pt-br/docs/setup/

Com o kubernetes instalado corretamente basta ir até a pasta k8s e executar o seguinte comando:
- `kubectl apply -f .`  (serão executados todos os arquivos da pasta iniciando suas configurações)
- `kubectl get deploy` (para ver os deploys que subiram, a API e o banco de dados no nosso caso);
- `kubectl get Pods` (para ver os respectivos pods)
- `kubectl get Svc` (para ver a configuração dos services dos pods, aqui você consegue pegar a porta ou endpoint para acessar seu container)

ex:
![image](https://github.com/user-attachments/assets/b336de8e-8774-4025-a8d3-256dc47317f8)

Caso esteja tendo dificuldades para acessar a respectiva porta você pode utilizar esse comando localmente para gerar um acesso em uma porta de sua escolha, basta abrir o cmd e executar:
`kubectl port-forward deployment/{nomeDeployment} 7003:80 7004:443`

Para finalizar os pods e os deploys você pode executar o seguinte comando:
`kubectl delete -f .`  (serão deletadas todas as configurações dos arquivos iniciados, finalizando assim os pods)


## Fases do projeto

### Fase 1:
Documentação do sistema (DDD) utilizando a linguagem ubíqua dos fluxos:
  
- Realização do pedido e pagamento

- Preparo e entrega do pedido

Link documentação: https://www.figma.com/file/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?type=whiteboard&t=eIOHebPJdDjUs6OT-1


Projeto com foco no backend seguindo os padrões solicitados em aula:

- `Arquitetura Hexagonal`

- `Api`
    - Cadastro de cliente
    - Identificação via CPF
    - Criar, editar e remover produtos
    - Buscar produtos por categoria
    - Fake checkout, apenas enviar os produtos escolhidos para a fila (futuramente será implementado um provedor de pagamento)
    - Listar pedidos
    - Aplicação com foco em escalabilidade

  
- `Banco de dados`
  - MongoDb
    
- `Docker e Docker-Compose`
  -  Criação e orquestração dos containers (1 para a aplicação e um para o banco de dados)


### Fase 2:
Atualização da aplicação desenvolvida na Fase 1 refatorando código para seguir os padrões de clean code e clean architecture;
- Checkout do pedido e identificação do mesmo

- Consultar status do pagamento

- API para confirmar pagamento do pedido ( postergando a necessidade de realizar a integração com provedor de pagamento) 

- Webhook para saber se o pagamento foi aprovado

- Ordenação de pedidos seguindo critérios de status e data

- Atualização de status do pedido


Projeto com foco no backend seguindo os padrões solicitados em aula:

- Arquitetura Limpa

- `Api TechChallenge-LanchoneteTotem`
  - Cadastro de cliente
  - Identificação via CPF
  - Criar, editar e remover produtos
  - Buscar produtos por categoria
  - Fake checkout, apenas enviar os produtos escolhidos para a fila (futuramente será implementado um provedor de pagamento)
  - Listar pedidos
      
- `Api Notifier`
  - Envio da confirmação do pagamento do pedido (utilizado pra simular o provedor de pagamento)
 
- `Webhook PagamentoPedidoNotificationConsumer`
  - Captura de confirmações de pagamento e atualização do status do pedido
 
- `Ferramentas e serviços`
  - LocalStack
  - AWS SQS
  - AWS SecretManager 

- `Docker e Docker-Compose`
  -  Criação e orquestração dos containers (4 Containers)
  - 1 Api TechChallenge-LanchoneteTotem
  - 1 Api Notifier
  - 1 Webhook PagamentoPedidoNotificationConsumer
  - 1 LocalStack
  - 1 Banco de dados

- `Estrutura Kubernetes`
  - Escalabilidade com aumento e diminuição de Podes conforme demanda
  - Arquivos manifesto (yaml)

- `Desenho da arquitetura`
  - Requisitos de negócio e requisitos de infraestrutura
  - Link documentação: https://www.figma.com/file/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?type=whiteboard&t=sPLhHH5Z8c1pNv9q-1
  - Demonstração da arquitetura: https://www.youtube.com/watch?v=rMTpeYQWRuE
 
  OBs: Dentro da pasta do projeto existe a pasta "Info" nela está a collection do projeto para ser utilizada no postman e um txt com o passo a passo e comandos para realizar a execução dentro do kubernetes

### Fase 3:

 Utilização da aplicação kubernetes desenvolvida na Fase 2 para o deploy na AWS utilizando terraform e GitHubActions.

1. Implementar um API Gateway e um function serverless para autenticar o cliente com base no CPF
   a. Integrar ao sitema de autenticação para identificar o cliente [Pendente]

2. Implementar as melhores práticas de CI/CD para a aplicação, segregando os códigos em repositórios:
   a. 1 repositório Banco de dados gerenciaveis com terraform:
     - https://github.com/WillianViegas/TechChallenge-LanchoneteTotem-Infra-Banco
       
   a. 1 repositório infra Kubernetes com terraform:
     - https://github.com/WillianViegas/TechChallenge-LanchoneteTotem-Infra
       
   c. 1 repositório Lambda + cognito:
    - https://github.com/WillianViegas/TechChallenge-LanchoneteTotem-Lambda
      
   d. 1 repositório para aplicação que é executada o kubernetes (Repositório atual)


3. Repositórios devem fazer deploy automatizado na conta da nuvem utilizando actions. As branches main/master devem ser protegidas não permitindo commits diretos. Obrigatório uso de pull request.


4. Melhorar estrutura do banco, documentar e justificar escolha do banco de dados:

![image](https://github.com/WillianViegas/TechChallenge-LanchoneteTotem/assets/58482678/46dd4524-9c78-416e-93d2-edd2a87e6a3a)

   - Documentação do Banco: https://www.figma.com/file/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?type=whiteboard&node-id=0%3A1&t=NrS7vAgBQheSdpmh-1

  
5. Obrigatoriedadde de utilização de serviços de algum provedor de cloud.
  - Cloud utilizada = AWS;
  - Principais serviços utilizados = Lambda, Cognito, ECR, EKS, EC2, IAM, DocumentDB, APIGateway;  

Video explicando a arquitetura desta fase: https://www.youtube.com/watch?v=TU74cMct8sk


### Fase 4:
1. Refatore o projeto separando em ao menos 3 microsserviços:
   - Pedido: Responsável por organizar o processo de pedidos;
     - rep: https://github.com/WillianViegas/techchallenge-microservico-pedido
    
   - Pagamento: Responsável por operacionalizar a cobrança do pedido;
      - rep: https://github.com/WillianViegas/techchallenge-microservico-pagamento

   - Produção: Responsável por operacionalizar o processo de produção do pedido;
      - rep: https://github.com/WillianViegas/techchallenge-microservico-producao

  Critérios obrigatórios:
   - Usar ao menos um banco NoSQL e um SQL;
   - Os serviços devem se comunicar por chamada direta, fila ou outras estratégias. Os serviços não podem acessar os bancos de dados uns dos outros;

2. Ao refatorar, os serviços devem conter testes unitários:
   - Ao menos um dos caminhos de teste deve implementar BDD
   - Em todos os projetos a cobertura de testes deve ser de 80%
  
3. Seus repositórios devem ser separados para cada aplicação e devem respeitar as seguintes regras:
   - Main protegida
   - PR para a branch main deve validar o build da aplicação e a qualidade do código via Sonarqube
   - Automatize o deploy dos seus microsserviços
  

Link do fluxograma + fluxos das fases anteriores:
https://www.figma.com/board/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?node-id=0%3A1&t=oY6vBdqPodcM5LMR-1

Link do video explicativo desta fase:
https://youtu.be/-OZgHsUoLkM


