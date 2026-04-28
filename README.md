# WinFIM Guard 🛡️

Bem-vindo ao repositório do **WinFIM Guard**! Este é um projeto pessoal focado em segurança defensiva, projetado para monitorar a integridade de arquivos em sistemas Windows (FIM - File Integrity Monitoring).

> **Nota de Desenvolvimento:** Este projeto foi totalmente arquitetado e desenvolvido com o auxílio do **Google Antigravity** (equipe DeepMind), seguindo uma metodologia de desenvolvimento orientada por especificações (Spec-Driven Development).

---

## 📌 O que é o projeto?

O WinFIM Guard é uma aplicação desktop autônoma, focada em monitoramento de integridade de arquivos (FIM). Ele permite que você crie uma "Baseline" (estado de referência inicial) de diretórios e, a partir daí, audite e monitore os arquivos tanto em **tempo real** quanto por meio de **revalidação manual** utilizando criptografia de hash (SHA-256).

### Principais Funcionalidades:
- Monitoramento em tempo real (identifica Criação, Modificação, Exclusão e Renomeação).
- Cálculo e comparação de Hashes (SHA-256) via Revalidação.
- Gerenciamento de múltiplos diretórios simultaneamente.
- Persistência local (100% offline, respeitando a privacidade).
- Exportação de logs de eventos para CSV.

---

## 🛠️ Tecnologias Utilizadas

A stack escolhida focou na estabilidade, independência de bibliotecas de terceiros complexas e forte aderência ao ecossistema Microsoft:
- **C# / .NET**
- **WPF (Windows Presentation Foundation)** para a interface gráfica moderna.
- **SQLite** para armazenamento embutido.
- **Entity Framework Core** como ORM.
- **CommunityToolkit.Mvvm** para arquitetura MVVM limpa.
- **Injeção de Dependências** nativa do .NET (Microsoft.Extensions.DependencyInjection).

---

## 🚀 Como o projeto foi construído (Passo a Passo)

A construção do projeto seguiu um modelo guiado de arquitetura e codificação usando inteligência artificial:

### Passo 1: Especificação de Requisitos de Software (ERS)
Tudo começou com um documento denso em Markdown detalhando os Requisitos Funcionais, Não Funcionais, Casos de Uso e Regras de Negócio de como um software FIM moderno deveria se comportar.

### Passo 2: O Plano de Implementação (Via Antigravity)
Em vez de começar a codar de forma aleatória, forneci o documento ERS ao **Antigravity**. O agente de IA atuou como arquiteto de software, extraindo do documento um *Plano de Implementação* contendo a visão arquitetural (Core, Data e UI), a stack técnica validada, o diagrama de banco de dados e as fases do projeto.

### Passo 3: Setup e Automação Inicial
Uma vez que eu aprovei o plano, a IA usou comandos de terminal automatizados (`dotnet new`, `dotnet add reference`) para separar a solução nas camadas corretas e instalar todas as bibliotecas do Nuget necessárias, garantindo que eu não perdesse tempo configurando dependências circulares e infraestrutura.

### Passo 4: Implementação do Core (O Coração do Software)
Iniciamos programando os modelos do banco de dados (Entidades) e os serviços isolados:
- Criamos o `HashingService` (calcula o SHA-256 dos arquivos).
- Construímos o `FileMonitorService`, que injeta instâncias da API nativa do Windows (`FileSystemWatcher`) para ouvir alterações no disco no momento exato em que ocorrem.
- Desenvolvemos o `BaselineService` e o `RevalidationService` (os mecanismos que tiram as "fotos" iniciais das pastas e comparam para encontrar modificações silenciosas furtivas).

### Passo 5: Camada de Persistência (Data)
Injetamos a interface no `WinFimDbContext` do Entity Framework e executamos a primeira Migration via linha de comando pelo agente, criando instantaneamente o arquivo `.db` do SQLite local.

### Passo 6: Interface Gráfica (WPF e MVVM)
Para finalizar, focamos em injetar a vida visual no software:
- Substituímos a inicialização padrão do WPF (`App.xaml`) por um sistema de `Host` global com Injeção de Dependência.
- Criamos os ViewModels (Dashboard, Baseline, Eventos e Diretórios) e lidamos com a ligação de dados (`{Binding}`) no XAML.
- O agente programou a navegação entre as telas conectando o Menu Lateral aos DataTemplates do framework.

O resultado é um software de cibersegurança limpo, modularizado e funcional, construído em tempo recorde através do pareamento humano + agente!

---

## 🖥️ Como rodar o projeto localmente

Como o sistema lida com criação autônoma de banco de dados e migrations, executar pela primeira vez é super simples. 

1. Certifique-se de ter o [SDK do .NET](https://dotnet.microsoft.com/) instalado no seu computador.
2. Clone este repositório ou baixe o diretório `WinFIM`.
3. Abra o terminal na raiz do projeto (`c:\Users\SeuNome\Caminho\WinFIM`).
4. Rode o seguinte comando:

```bash
dotnet run --project WinFIM.UI
```

A aplicação fará o build, aplicará a estrutura do banco SQLite localmente de modo transparente e abrirá a interface automaticamente.

---
*Este é um projeto de caráter pessoal e de estudo criado por Bhrenno Borges com assistência do agente Antigravity.*
