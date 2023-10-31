
# TechChallenge-LanchoneteTotem

Sistem de solicitação de pedido via totem de autoatendimento para lanchonete criado em .Net 7 utilizando Minimal Api e mongoDB, seguindo o padrão de arquitetura hexagonal. Proposta realizada como meio avaliativo da Fiap Pos Tech 

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
Após a subida dos containers basta acessar: https://localhost:7004/swagger/index.html

### Portas
Aplicação:
    
    7004:443
    
    7003:80

MongoDb:

    27017:27017

 ## Populando o banco de dados para os dados iniciais

 Existe uma requisição para popular o banco num primeiro momento, criando 2 usuários e algumas categorias e produtos para facilitar o entendimento inicial do sistema. 
 
 Você pode acessa-lá pelo swagger ou fazer a requisição você mesmo. (obs. ela só faz a inserção caso as tabelas de usuário, categoria ainda estejam vazias); 

    endpoint + /seed


 ## Proposta atual

 Documentação do sistema (DDD) utilizando a linguagem ubíqua dos fluxos:
  
- Realização do pedido e pagamento

- Preparo e entrega do pedido

Link documentação: https://www.figma.com/file/foY2Q9t6aj6Gzv9WK8actk/Documenta%C3%A7%C3%A3o-Sistema-DDD?type=whiteboard&t=eIOHebPJdDjUs6OT-1


Projeto com foco no backend seguindo os padrões solicitados em aula:

- Arquitetura Hexagonal

- Api
    - Cadastro de cliente
    - Identificação via CPF
    - Criar, editar e remover produtos
    - Buscar produtos por categoria
    - Fake checkout, apenas enviar os produtos escolhidos para a fila (futuramente será implementado um provedor de pagamento)
    - Listar pedidos

- Aplicação com foco em escalabilidade
- Banco de dados (realizado com mongoDb)
- Utilização de docker e dockercompose para a criação e orquestração dos containers (1 para a aplicação e um para o banco de dados)


## Futuras adições e melhorias:
    - Implentar meio de autenticação (foi iniciado alguns testes com JWT para entendimento da tecnologia mas deve ser melhor desenvolvido futuramente )
    - Adicionar Logs no projeto
    - Deixar respostas das requisições mais agradáveis
    - Implementar meio de pagamento do Mercado pago
    - Implementar testes de unidade
    - Melhorar e corrigir detalhes da Arquitetura