# 🎯 Point Blank Server v3.68

> **Emulador de servidor privado para o clássico FPS Point Blank**
>
> Projeto educacional e comunitário — sem vínculo com a Zepetto ou desenvolvedores oficiais.

---

## 📋 Sobre o Projeto

Este repositório contém o **código-fonte completo do servidor** do Point Blank, incluindo scripts de banco de dados, ferramentas e arquivos de configuração necessários para executar um servidor customizado do jogo.

O servidor é dividido em **4 projetos** que se comunicam entre si:

| Projeto | Tipo | Função | Porta Padrão |
|---------|------|--------|-------------|
| **PointBlank.Core** | 📦 Biblioteca (DLL) | Código compartilhado: banco, XML, modelos, gerenciadores | — |
| **PointBlank.Auth** | 🚪 Aplicação (EXE) | Servidor de autenticação e login | `39190` |
| **PointBlank.Game** | 🎮 Aplicação (EXE) | Servidor de jogo (salas, clans, shop) | `39191` |
| **PointBlank.Battle** | ⚔️ Aplicação (EXE) | Servidor de batalha em tempo real (UDP) | `40009` |

### Arquitetura de Comunicação

```
Cliente PB
    │
    ├──► Auth (TCP :39190) ───── Sincronia (:1908) ──┐
    │                                                 │
    ├──► Game (TCP :39191) ───── Sincronia (:1909) ──┤
    │                                                 │
    └──► Battle (UDP :40009) ─── Sincronia (:1910) ──┤
                                                      │
                                                      ▼
                                           ┌─────────────────┐
                                           │   PostgreSQL    │
                                           │  (Banco de Dados)│
                                           └─────────────────┘
```

---

## ✨ Funcionalidades

### 🔐 Autenticação
- Login de jogadores com criptografia
- Verificação de versão do cliente (`DirectX.xml`)
- Suporte a múltiplos servidores de jogo
- Criação automática de contas (opcional)
- Multi-idiomas: Turkey, MiddleEast, Brazil, Thai, Russia, Indonesia

### 🎮 Jogo
- **Salas**: Criação, join, quick join, customização (mapa, regras, armas)
- **Modos de jogo**: Deathmatch, Bomb/C4, Dino, TDM, Defuse, Conquest, HeadHunter, etc.
- **Sistema de clans**: Criação, hierarquia (mestre, oficial, membro), guerras de clans
- **Shop**: Loja completa com itens por tempo/permanente, gift, coupons
- **Inventário**: Gerenciamento de itens, equipamento, repair
- **Missões**: Sistema de cartas de missão com recompensas
- **Títulos**: Sistema de conquistas e títulos
- **Ranking**: Experiência (EXP), pontos (GP), taxa de vitórias
- **Anti-Kick**: Sistema de votação para kick
- **RCON**: Comandos remotos via WebSocket (Fleck)

### ⚔️ Batalha (UDP)
- Simulação de combate em tempo real via **Lidgren.Network**
- Sincronização de: dano, morte, C4, portal, hit markers
- Suporte a bots (AI)
- Logs de batalha

### 🛠️ Comandos Administrativos
| Categoria | Comandos |
|-----------|----------|
| Moderador | Nick history, kick, anti-kick, ping, AFK |
| GameMaster | Ban, mute, gift, anuncio global, troca de mapa |
| Administrador | Ban eterno, desbanir, set cash/gold, set VIP |
| Desenvolvedor | Reload shop, give item, trocar de nick |

---

## 🚀 Começando

### 📦 Pré-requisitos

| Software | Versão | Onde obter |
|----------|--------|------------|
| **Visual Studio 2022** | Community (gratuito) ou superior | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| **.NET Framework 4.8** | SDK + Developer Pack | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet-framework/net48) |
| **PostgreSQL** | 14, 15, 16 ou 17 | [postgresql.org](https://www.postgresql.org/download/) |
| **Git** | Qualquer versão recente | [git-scm.com](https://git-scm.com/) |

> **Workload do VS:** Durante a instalação, selecione **"Desenvolvimento para desktop com .NET"**.

### 🔧 Instalação Passo a Passo

#### 1️⃣ Clone o repositório

```bash
git clone https://github.com/dekthaiinchina/PointBlankServer.git
cd PointBlankServer
```

#### 2️⃣ Configure o PostgreSQL

**a) Acesse o PostgreSQL:**
```powershell
psql -U postgres
```

**b) Crie o banco de dados:**
```sql
CREATE DATABASE postgres;
```

**c) Importe o schema e dados:**
```powershell
psql -U postgres -d postgres -f "PointBlankServer\pointblank.sql"
```

**d) Configure a senha do usuário postgres:**
```sql
ALTER USER postgres PASSWORD '123456';
```

**e) Verifique o arquivo `pg_hba.conf`** (geralmente em `C:\Program Files\PostgreSQL\17\data\pg_hba.conf`):
```
host    all    all    127.0.0.1/32    md5
```
> ⚠️ Após alterar, **reinicie o serviço PostgreSQL**.

#### 3️⃣ Configure os arquivos INI

Edite os arquivos em `build\rel\Config\` (ou `build\dbg\Config\` para Debug):

**`Database.ini`** — Configuração de banco (usado por todos os servidores):
```ini
; Database Settings
Host=localhost
Name=postgres
User=postgres
Pass=123456          ← Sua senha do PostgreSQL
Port=5432
EncodingPage=874
```

**`Auth.ini`** — Servidor de autenticação:
```ini
[all]
AuthIp=127.0.0.1
AuthPort=39190
SyncPort=1908
Debug=true
Test=false
AutoAccounts=false
GameLocales=Turkey,MiddleEast,Brazil,Thai,Russia,Indonesia
```

**`Game.ini`** — Servidor de jogo:
```ini
[all]
GameIp=127.0.0.1
GamePort=39191
SyncPort=1909
Debug=true
MaxChannelPlayers=100
MaxBattleXP=2000
MaxBattlePoint=2000
UdpType=3
```

**`Battle.ini`** — Servidor de batalha:
```ini
UdpIp=127.0.0.1
ServerIp=127.0.0.1
UdpPort=40009
SyncPort=1910
UdpVersion=1508.7
Test=true
MaxDrop=25
```

#### 4️⃣ Compile a Solução

**Pelo Visual Studio:**
1. Abra `PointBlankServer.sln`
2. Selecione **"Release"** (ou "Debug" para testes)
3. **Build > Build Solution** (Ctrl+Shift+B)

**Pela linha de comando (MSBuild):**
```powershell
msbuild PointBlankServer.sln /p:Configuration=Release
```

#### 5️⃣ Execute os Servidores

**Opção A — Manual (3 terminais separados):**

```powershell
REM Terminal 1 - Auth
cd PointBlankServer\build\rel
PointBlankAuth.exe

REM Terminal 2 - Game
cd PointBlankServer\build\rel
PointBlankGame.exe

REM Terminal 3 - Battle
cd PointBlankServer\build\rel
PointBlankBattle.exe
```

**Opção B — Script automático:**
```
StartServer.cmd
```

> ⚠️ **Ordem obrigatória:** Auth → Game → Battle

---

## 📁 Estrutura do Projeto

```
PointBlankServer/
│
├── PointBlankServer.sln          ← Solução do Visual Studio
├── pointblank.sql                ← Script completo do banco PostgreSQL
├── README.md                     ← Este arquivo
│
├── PointBlank.Core/              ← ☕ Biblioteca central (DLL)
│   ├── ConfigFile.cs             ← Leitor de arquivos .ini
│   ├── Config.cs                 ← Variáveis estáticas de configuração
│   ├── Logger.cs                 ← Sistema de logs
│   ├── Sql/
│   │   └── SqlConnection.cs      ← Singleton de conexão PostgreSQL
│   ├── Xml/                      ← Parsers de dados XML
│   ├── Network/                  ← Protocolo de rede (packets)
│   ├── Models/                   ← Modelos de dados
│   └── Managers/                 ← Gerenciadores (eventos, shop, etc.)
│
├── PointBlank.Auth/              ← 🚪 Servidor de Autenticação
│   ├── Program.cs                ← Ponto de entrada
│   ├── Auth.cs                   ← Loop principal
│   ├── AuthManager.cs            ← Gerenciador de conexões
│   ├── Data/
│   │   ├── Configs/AuthConfig.cs ← Carregador de Auth.ini
│   │   └── Xml/ChannelsXml.cs    ← Canais via banco
│   └── Network/                  ← Packets de rede
│
├── PointBlank.Game/              ← 🎮 Servidor de Jogo
│   ├── Program.cs                ← Ponto de entrada
│   ├── Game.cs                   ← Loop principal
│   ├── GameManager.cs            ← Gerenciador de conexões
│   ├── Data/
│   │   ├── Configs/GameConfig.cs ← Carregador de Game.ini
│   │   ├── Model/                ← Account, Room, Channel, Match
│   │   ├── Xml/                  ← BattleServerXml, ChannelsXml
│   │   ├── Managers/             ← Account, Clan, GameRule
│   │   ├── Chat/                 ← Comandos administrativos
│   │   ├── Command/              ← Sistema de comandos via chat
│   │   └── Sync/                 ← Sincronização com Battle
│   └── Network/                  ← Packets cliente/servidor
│
├── PointBlank.Battle/            ← ⚔️ Servidor de Batalha
│   ├── Program.cs                ← Ponto de entrada
│   ├── Data/
│   │   ├── Configs/BattleConfig.cs ← Carregador de Battle.ini
│   │   ├── ConfigFile.cs         ← Leitor de .ini (versão Battle)
│   │   └── Sql/SqlConnection.cs  ← Conexão própria (não usa Core)
│   └── Network/                  ← Comunicação UDP
│
├── build/
│   ├── dbg/                      ← Build Debug
│   │   ├── Config/               ← Configurações (Database.ini, etc.)
│   │   │   └── Translate/        ← Arquivos de tradução
│   │   ├── Data/                 ← Dados do jogo (XML)
│   │   │   ├── Battle/           ← Charas.xml, Exceptions.xml, Maps.xml
│   │   │   ├── Cards/            ← Recompensas de missão
│   │   │   ├── Coupons/          ← Cupons da loja
│   │   │   ├── Filters/          ← Filtro de nicknames
│   │   │   ├── Maps/             ← Regras de mapas
│   │   │   ├── Missions/         ← Cartas de missão (.mqf)
│   │   │   ├── Rank/             ← EXP por rank
│   │   │   ├── Shop/             ← Shop.dat (loja)
│   │   │   └── Titles/           ← Títulos e conquistas
│   │   ├── Encryption.dll        ← DLL de criptografia
│   │   ├── PointBlankAuth.exe
│   │   ├── PointBlankGame.exe
│   │   └── PointBlankBattle.exe
│   │
│   └── rel/                      ← Build Release (mesma estrutura)
│
└── packages/                     ← Dependências NuGet
    ├── Npgsql.8.0.7/             ← Conector PostgreSQL
    ├── Lidgren.Network.1.0.2/    ← Rede UDP
    ├── SharpDX.4.2.0/            ← DirectX (matemática)
    ├── Fleck.1.2.0/              ← WebSocket (RCON)
    └── CrashReporter.NET.1.6.0/  ← Relatório de crashes
```

---

## 🗄️ Banco de Dados (PostgreSQL)

### Tabelas Principais

| Tabela | Descrição |
|--------|-----------|
| `accounts` | Contas de jogadores (login, senha, IP) |
| `players` | Dados dos personagens (nick, rank, exp, gold, cash) |
| `player_items` | Itens no inventário de cada jogador |
| `player_characters` | Personagens/avatares dos jogadores |
| `player_friends` | Lista de amigos |
| `player_messages` | Mensagens entre jogadores |
| `clans` | Clãs (nome, logo, membros, pontos) |
| `clan_invites` | Convites para clãs |
| `channels` | Canais de jogo (Novice, General, Clan) |
| `gameservers` | Servidores de jogo registrados |
| `item_goods` | Catálogo da loja (400+ itens) |
| `ban_history` | Histórico de banimentos |
| `ban_hwid` | Banimentos por hardware |
| `server_events`, `server_cards_awards`, `server_coupons` | Eventos, recompensas e cupons |

### Portas e Sincronia

| Servidor | Porta TCP | Porta Sync | Finalidade |
|----------|-----------|------------|------------|
| Auth | 39190 | 1908 | Login, lista de servidores |
| Game | 39191 | 1909 | Jogo, salas, clans, shop |
| Battle | 40009 (UDP) | 1910 | Combate em tempo real |

---

## 🔍 Solução de Problemas

### ❌ Erro: `28P01: password authentication failed`

**Causa:** A senha do PostgreSQL está incorreta nos arquivos .ini ou o método de autenticação está errado.

**Solução:**
1. Confirme se está editando o arquivo certo (`build\rel\Config\Database.ini` ou `build\dbg\Config\Database.ini`)
2. Redefina a senha:
   ```sql
   ALTER USER postgres PASSWORD '123456';
   ```
3. Verifique `pg_hba.conf`:
   ```
   host    all    all    127.0.0.1/32    md5
   ```
4. Reinicie o PostgreSQL

### ❌ Erro: "Falha ao conectar — porta em uso"

**Causa:** Outro processo já está usando a porta.

**Solução:**
```powershell
netstat -ano | findstr :39190
taskkill /PID <PID> /F
```

### ❌ Erro: `File not found: Data/Battle/ServerList.xml`

**Causa:** O executável não está sendo rodado do diretório correto.

**Solução:** Execute sempre a partir de `build\rel\` ou `build\dbg\`.

---

## 📜 Licença e Aviso Legal

**Licença:** [MIT License](https://opensource.org/licenses/MIT)

**Aviso:** Este projeto é **exclusivamente para fins educacionais e de estudo**. Não é afiliado, endossado ou patrocinado pela **Zepetto** ou qualquer desenvolvedor oficial do Point Blank. Não utilize este código para fins comerciais, pois pode violar direitos de propriedade intelectual.

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para:

1. **Fork** o repositório
2. Crie uma **branch** para sua feature (`git checkout -b feature/minha-feature`)
3. Faça **commit** das alterações (`git commit -m 'Minha nova feature'`)
4. Faça **push** para a branch (`git push origin feature/minha-feature`)
5. Abra um **Pull Request**

---

## 📞 Contato & Comunidade

- **Repositório original:** [github.com/dekthaiinchina/PointBlankServer](https://github.com/dekthaiinchina/PointBlankServer)

---

<p align="center">
  <i>Desenvolvido com ❤️ pela comunidade — para a comunidade</i>
</p>
