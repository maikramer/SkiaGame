# SkiaGame

> Este é um projeto para melhorar o aprendizado na linguagem C# e talvez aprender algo mais, ela utilizar SkiaSharp e pode executar algumas operações de desenho em 2D básicas, as mesmas suportadas em SkiaSharp.

## 1.0. Fisica 2D

A Engine tem fisica 2D com as seguintes capacidades:

* Objetos podem ter Gravidade própria, de acordo com suas massas e distancias de seu centro (Gravitação de Newton).
* Objetos podem sofrer Gravidade global (Porém em Pixels por segundo).
* Objetos sofrem Atrito global.
* Objetos colidem (Circulos e Retangulos Somente)
  

<img src="https://github.com/maikramer/SkiaGame/raw/main/Fisica.gif" width="800" height="600" />

## 1.1 Ajustes e melhorias

O projeto ainda está em desenvolvimento e as próximas atualizações serão voltadas nas seguintes tarefas:

- [x] Fazer Rodar em GTK (Linux)
- [x] Fazer Rodar em Maui(Testado no Android por Enquanto)
- [x] Implementar física 2D básica
- [x] Controles Touch (Somente GTK no momento)
- [ ] Elaborar mais funções

## 1.2. 💻 Pré-requisitos

Antes de começar, verifique se você atendeu aos seguintes requisitos:
* Você vai precisar instalar o `.NET6.0` para rodar na forma mais básica.
* Para rodar as versões móveis, além dele vai precisar também instalar as `workloads`, explicadas abaixo.

## 1.3. 🚀 Clonando o SkiaGame

O primeiro Passo seria instalar o Git. É fácil e já existem diversos tutoriais de como clonar um repositório, más, o comando será.

Para clonar o SkiaGame, siga estas etapas:

- Abra o `cmd` ou `terminal`.
- Navegue até a pasta onde quer que fique a pasta com os arquivos.

Windows, Linux e macOS:
```
git clone git@github.com:maikramer/SkiaGame.git
```

## 1.4. ☕ Instalando Workloads

Você vai precisar instalar os arquivos do android se quiser instalar no seu celular android. Eu não vou explicar aqui mas existem tutoriais, porém, o código que rodaria no android é exatamento o mesmo do `maui-windows`.
Então para rodar no windows a versão Maui, basta instalar a workload dele.
```
dotnet workload install maui
```
A Workload Android Maui pode ser instalada mas acredito que não será o único requisito.

```
dotnet workload install maui-android
```

Adicione comandos de execução e exemplos que você acha que os usuários acharão úteis. Fornece uma referência de opções para pontos de bônus!

## 1.5. 📫 Contribuindo para SkiaGame
Para contribuir com SkiaGame, siga estas etapas:

1. Bifurque este repositório.
2. Crie um branch: `git checkout -b <nome_branch>`.
3. Faça suas alterações e confirme-as: `git commit -m '<mensagem_commit>'`
4. Envie para o branch original: `git push origin SkiaGame / <local>`
5. Crie a solicitação de pull.

Como alternativa, consulte a documentação do GitHub em [como criar uma solicitação pull](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/creating-a-pull-request).

## 1.6. 🤝 Colaboradores

* Por enquanto, só eu.. sozinho ..

## 1.7. 😄 Seja um dos contribuidores<br>

Quer fazer parte desse projeto? Abra seu primeiro Pull Request e comece.

## 1.8. 📝 Licença

Esse projeto está sob licença. Veja o arquivo [LICENÇA](LICENSE) para mais detalhes.

[⬆ Voltar ao topo](#nome-do-projeto)<br>

