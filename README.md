# Banco-de-Dados
Olá Professor, 
Nosso grupo tem u m pouco de dificuldades de usar o github, por isso os commits não sao tao detalhados e parece que da última entrega deu a entender que a divisão de trabalho não foi justa.
Nosso grupo faz o trabalho sempre junto e em ligação, e sempre que um tópico é feito uma pessoa sobe um commit, por sempre fazermos em call um ajuda o outro e vice e versa, como o trabalho é em grupo ele é feito em grupo também, ao contrario de um faz uma parte sozinho e envia assim por diante.

Nessa entrega há cinco commits principais:

 - Ajuste dos erros da entrega passada: 
	Houve participação de todos os integrantes, os ajustes implementados foram -> Validação de 	entrada, Tratamento de exceções, Feedback ao Usuário, Adição de comentários.

 - Criação de dois programas e sua ligação: 
	 Criação de dois programas para separa database server e o data base cliente. Usamos o IPC 	criamos a comunicação entre os dois scripts DataBaseServer.cs e DatabaseClient.cs, usamos o 	Named Pipes. O servidor aguarda a conexao do cliente e processa as solicitações recebidas. O 	cliente conecta ao servidor e envia solicitaçoes ao servidor.

 - Implementação de RNF-1, RNF-2, RNF-3:
	Os requisitos não funcionais listados foram implementados:
	• RFN-1 Suportar uma interface por linha de comandos para realizar operações diretas no
	banco de dados.
	• RFN-2:1 Suportar a comunicação bidirecional com o programa cliente usando algum me-
	canismo de comunicação entre processos.
	• RFN-3 Suportar o processamento concorrente de requisições no banco de dados, usando
	threads.

 - Implementação de RFN-7 e RFN-8
	os seguintes requisitos não funcionais foram implementados ao programa cliente do banco de 	dados:
	• RFN-7: Ler as requisições do dispositivo de entrada padrão.
	• RFN-8: Escrever a resposta recebida das requisições feitas ao banco de dados no disposi-
	tivo de saída padrão.
- Ajustes necessários
	 Aqui foram feitos ajustes como no primeiro tópico, foram revisados e testados.


Alunos: Sara Mendoza, Saulo Macedo, Hideki Ishi