<h1 align="center"> Store Back-End </h1>



## Índice 

[Índice](#índice)

[Descrição do Projeto](#descrição-do-projeto)

[Funcionalidades](#funcionalidades)

[Acesso ao Projeto](#acesso-ao-projeto)

[Usando o Projeto](#usando-o-projeto)

[Exemplos de Busca](#exemplos-de-busca)

[Tecnologias utilizadas](#tecnologias-utilizadas)


## Descrição do Projeto
  Projeto back-end para cadastrar, deletar. atualizar e listar produtos armazenando-os em uma base de dados azure.
  Link para testar a aplicação online: https://lojinha-api.azurewebsites.net/index.html

## Funcionalidades
-   Todos os end-points estão documentados via Swagger e são mostrados ao rodar o projeto.
-   Auth: Contem os end-poits relacionados ao login e autorização para uso da API
    - Signin: Responsável por logar na api e fornecer o token de acesso
    - Refresh: Atualiza o token após expiração
    - Revoke: Revoga o token do usuario
-   Produto: Contem os end-points relacionados aos produtos
    -   Post loja/produto: Cadastra um novo produto
    -   Get loja/Produto: Retorna uma lista de todos os produtos salvos na base de forma paginada ao consultar pelo nome
    -   Put loja/Produto: Atualiza o produto na base
    -   Get loja/Produto/{produtoid}: Busca na base um produto pelo ID
    -   Get loja/Produto/findAll: Retorna todos os produtos na base de dados
    -   Delete loja/Produto/{id}: Deleta um produto pelo Id
    
## Acesso ao projeto

Para acessar o projeto, basta fazer download desse [repositório](https://github.com/wesleyluz/Store-Backend_CSharp).

## 🛠️ Abrir e rodar o projeto
Esse é um projeto C#, utilizando o Framework .NET e Entity, consultando de uma base de dados hospedada na Azure. 
Para acessar o projeto, basta abri-lo na sua IDE favorita.
Caso queria usar um banco de dados local, basta criar sua base de dados com o nome Loja e atualizar o valor de 'MSSQLServerConnectionString'
por sua nova string de conexão neste [arquivo](https://github.com/wesleyluz/Store-Backend_CSharp/blob/main/lojinha-backend/appsettings.json), 
no [Startup](https://github.com/wesleyluz/Store-Backend_CSharp/blob/main/lojinha-backend/Startup.cs) procure por Migrations e remova os comentários do if.
Pronto agora é só rodar, basta executar e acessar os end-points no seu navegador ou no PostMan.

## Usando o Projeto
Ao rodar o projeto a página do swagger irá se abrir, caso sua IDE não abra automatico acesse: https://localhost:44390/swagger/index.html

  - A principio a pagina incial será assim:
<img src="https://raw.githubusercontent.com/wesleyluz/Store-Backend_CSharp/main/Tutorial_img/Home.jpeg" width=max-width height="500">

  - Abra a aba de Auth/signin para fazer o login, aperte em try it out, altere o valor de userName de "string" para "admin" e o valor de password para "admin123" depois execute
<img src="https://raw.githubusercontent.com/wesleyluz/Store-Backend_CSharp/main/Tutorial_img/Logar.gif" width=max-width height="500">

  - Após logar, copie o acessToken gerado:
<img src="https://raw.githubusercontent.com/wesleyluz/Store-Backend_CSharp/main/Tutorial_img/Copiar_token.gif" width=max-width height="500">

  - Por fim vá em Authorize na caixa digite "Bearer", dê um espaço e cole o token clique em Authorize e estará autenticado:
<img src="https://raw.githubusercontent.com/wesleyluz/Store-Backend_CSharp/main/Tutorial_img/Autenticar.gif" width=max-width height="500">

Pronto, agora todas as consultas estarão liberadas para serem feitas via swagger, caso queira fazer via postman ou insomnia, 
basta passar através do header a chave Authorization com os valores Bearer e sua chave token quando for fazer uma consulta.

- Para facilitar o uso da API no Postman, crie um Environment e antes de fazer o login, vá na aba Tests e cole o seguinte código :

```
if(responseCode.code >=200 && responseCode.code <= 299)
{
    var jsonData = JSON.parse(responseBody);
    postman.setEnvironmentVariable('accessToken', jsonData.acessToken);
    postman.setEnvironmentVariable('refreshToken', jsonData.refreshToken);

}
```

## Exemplos de Busca
- Consultar produtos pelo nome com retorno paginado:
<img src="https://raw.githubusercontent.com/wesleyluz/Store-Backend_CSharp/main/Tutorial_img/consulta_paged.gif" width=max-width height="400">

- Consultar todos os produtos:
<img src="https://raw.githubusercontent.com/wesleyluz/Store-Backend_CSharp/main/Tutorial_img/Consultar_tudo.gif" width=max-width height="400">

## Tecnologias utilizadas
-   `Linguagem`:C#      
-   `FrameWorks`: .NET, Entity
-   `Base de Dados`: Azure SQL
-   `IDE`: Visual Studio 2022.
