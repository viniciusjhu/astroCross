================================================================================
                            ASTROCROSS - PROJETO FINAL
================================================================================

DESENVOLVEDORES:
1. Flávia Marcella Gonçalves Moreira
2. Larissa Rodrigues de Ávila
3. Vinícius Ribeiro da Silva do Carmo

--------------------------------------------------------------------------------
DESCRIÇÃO DO PROJETO:
AstroCross é um jogo 3D no estilo "Endless Runner" (Corrida Infinita) com temática 
Sci-Fi. O jogador controla uma nave espacial que deve atravessar três fases 
distintas, cada uma com mecânicas e desafios únicos, conectados por portais.

FUNCIONALIDADES PRINCIPAIS:
- Sistema de Pontuação Global (Score) persistente entre fases.
- Transição fluida de cenas via Portais.
- Fase 1 (Larissa): Corrida terrestre com desvio de obstáculos e asteroides.
- Fase 2 (Vinícius): Voo espacial com mecânica de esquiva 3D, drones inimigos 
  com IA de perseguição e tiro, e asteroides ambientais/letais.
- Fase 3 (Flávia): Desafios de plataforma, obstáculos móveis e mecânica de 
  pontuação rígida (não pontua ao recuar).
- Menu Principal e Sistema de Game Over integrados.
- Sistema de Pause funcional (Tecla ESC) em todas as fases.

--------------------------------------------------------------------------------
FERRAMENTAS NECESSÁRIAS:
- Unity Engine (Versão recomendada: 2022.3 LTS ou superior).
- Visual Studio 2019/2022 ou Visual Studio Code (para edição de scripts).
- Git (para controle de versão).

REQUISITOS MÍNIMOS DE HARDWARE (Para Execução):
- Sistema Operacional: Windows 10 ou 11 (64-bit).
- Processador: Intel Core i3 / AMD Ryzen 3 ou superior.
- Memória RAM: 4 GB (8 GB recomendado).
- Placa de Vídeo: Compatível com DirectX 11 (Ex: NVIDIA GTX 660 / AMD Radeon HD 7870).
- Espaço em Disco: Aprox. 1 GB livre.

--------------------------------------------------------------------------------
INSTRUÇÕES PARA COMPILAR E EXECUTAR (BUILD):

1. Abra o Unity Hub e adicione a pasta descompactada do projeto.
2. Abra o projeto no Unity Editor.
3. Vá no menu "File" -> "Build Settings".
4. Certifique-se de que as cenas estejam adicionadas na lista "Scenes In Build" 
   exatamente nesta ordem de índices:
   
   [0] - MainMenu (ou Menu)
   [1] - MainLevel (Fase 1 - Larissa)
   [2] - Level2    (Fase 2 - Vinícius)
   [3] - Level3    (Fase 3 - Flávia/Cena Final)

   *Caso as cenas não estejam na lista, localize-as na pasta Assets e arraste-as.

5. Selecione a plataforma "Windows, Mac, Linux".
6. Em "Target Platform", escolha "Windows".
7. Architecture: "x86_64" (64-bit).
8. Clique em "Build".
9. Escolha uma pasta vazia para salvar o executável (ex: "Build").
10. Após a conclusão, abra a pasta e execute o arquivo "AstroCross.exe".

--------------------------------------------------------------------------------
OBSERVAÇÕES:
Este pacote contém apenas os arquivos fonte essenciais (Assets, Packages, 
ProjectSettings) para reduzir o tamanho do arquivo, conforme solicitado. 
A pasta 'Library' será gerada automaticamente pelo Unity na primeira abertura.
