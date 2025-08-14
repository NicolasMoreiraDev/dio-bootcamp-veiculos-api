***API de Gerenciamento de Veículos - Desafio DIO***


**📖 Sobre o Projeto**

Esta API REST foi desenvolvida como projeto para o Bootcamp de .NET da Digital Innovation One (DIO). O objetivo foi construir uma aplicação robusta utilizando o padrão Minimal API do ASP.NET Core, aplicando conceitos essenciais de desenvolvimento back-end, como:

Arquitetura de software em camadas (API, Domínio, Infraestrutura).

Autenticação e autorização seguras com JWT (JSON Web Tokens).

Persistência de dados com Entity Framework Core e MySQL.

Segurança de senhas com hashing (BCrypt).

Criação de testes de unidade, persistência e integração.

A aplicação permite o gerenciamento de uma frota de veículos, com diferentes níveis de acesso para administradores e editores.

Este projeto foi desenvolvido com base no repositório original fornecido pelo expert da DIO, disponível em: https://github.com/digitalinnovationone/minimal-api

**✨ Funcionalidades**

[✔] Autenticação Segura: Sistema de login com JWT para proteger os endpoints.

[✔] Hashing de Senhas: As senhas dos administradores são armazenadas de forma segura usando BCrypt.

[✔] Controlo de Acesso por Perfil (Roles): Endpoints restritos para perfis de Adm e Editor.

[✔] Operações CRUD para Veículos: Funcionalidades completas para Criar, Ler, Atualizar e Apagar veículos.

[✔] Testes Abrangentes: O projeto inclui testes de unidade, persistência (com BD em memória) e de request (integração).

[✔] Documentação com Swagger: A API é totalmente documentada e testável via Swagger UI.

**🚀 Tecnologias Utilizadas**

.NET 8

ASP.NET Core Minimal API

Entity Framework Core 8

MySQL (com o provider Pomelo.EntityFrameworkCore.MySql)

JWT (JSON Web Tokens) para autenticação

BCrypt.Net para hashing de senhas

MSTest para testes

Swagger (OpenAPI) para documentação

**Endpoints da API**

Método	Endpoint	Proteção	Descrição	Corpo do Exemplo (JSON)
POST	/administradores/login	🔓 Público	Autentica um utilizador e retorna um token JWT.	{ "email": "adminstrador@teste.com", "senha": "123456" }
POST	/veiculos	🔒 Adm, Editor	Cria um novo veículo.	{ "nome": "Uno", "marca": "Fiat", "ano": 2005 }
GET	/veiculos	🔓 Público	Lista todos os veículos (suporta paginação).	N/A
GET	/veiculos/{id}	🔓 Público	Obtém os detalhes de um veículo específico.	N/A
PUT	/veiculos/{id}	🔒 Adm, Editor	Atualiza os dados de um veículo existente.	{ "nome": "Uno Mille", "marca": "Fiat", "ano": 2006 }
DELETE	/veiculos/{id}	🔒 Adm	Apaga um veículo.	N/A
