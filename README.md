# Guia de Instalação e Execução do Projeto

Este projeto consiste em uma aplicação com backend desenvolvido em .NET 8 e frontend em React, utilizando Chakra UI v2 e Axios. Siga os passos abaixo para configurar e executar corretamente o projeto.

---

## Pré-requisitos

Certifique-se de ter as seguintes ferramentas instaladas:

- **.NET 8 SDK**
- **Node.js** (versão 16 ou superior)
- **npm** ou **yarn**
- **Banco de dados** configurado conforme sua necessidade (por exemplo, SQL Server, MySQL, etc.)

---

## Passos para Configuração e Execução

### 1. Configurando o Backend

1. **Navegue até o diretório raiz do backend**:
   ```bash
   cd YouTubei9.Services.VideoAPI
   ```

2. **Configure a string de conexão com o banco de dados**:
   - Abra o arquivo `appsettings.json`.
   - Atualize a propriedade `ConnectionStrings` conforme necessário. Exemplo:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;"
     }
     ```

3. **Execute as migrações** para preparar o banco de dados:
   ```bash
   dotnet ef database update
   ```

4. **Inicie o servidor**:
   ```bash
   dotnet run
   ```

   O backend estará disponível na porta configurada (geralmente `https://localhost:5001`).

---

### 2. Configurando o Frontend

1. **Navegue até o diretório do frontend**:
   ```bash
   cd YouTubei9.Services.VideoAPI/FrontEnd
   ```

2. **Instale as dependências**:
   ```bash
   npm install
   ```

3. **Configure o endpoint da API**:
   - Abra o arquivo `src/components/App.js` (ou onde o Axios está configurado).
   - Atualize o endpoint base para corresponder ao endereço do backend.
     ```javascript
     const api = axios.create({
       baseURL: "https://localhost:5001/api"
     });
     ```

4. **Inicie o servidor de desenvolvimento**:
   ```bash
   npm start
   ```

   O frontend estará disponível em `http://localhost:3000`.

---

## Estrutura do Projeto

- **Backend**:
  - `Controllers`: Contém os controladores da API.
  - `Data`: Inclui o contexto e migrações do banco de dados.
  - `Models`: Define os modelos de dados.
  - `Services`: Implementa a lógica de negócio.

- **Frontend**:
  - `public`: Contém arquivos públicos como `index.html`.
  - `src/components`: Contém os componentes React.
  - `src/index.js`: Arquivo principal do React.

---

## Observações

- Certifique-se de que o banco de dados está acessível e que as credenciais estão corretas.
- Para ambientes de produção, configure variáveis de ambiente para armazenar informações sensíveis, como a string de conexão e URLs da API.
- Caso encontre problemas, verifique os logs no terminal para identificar possíveis erros.

---

Pronto! Seu projeto está configurado e em execução. Em caso de dúvidas, consulte a documentação oficial das tecnologias utilizadas ou entre em contato com o responsável pelo projeto.

