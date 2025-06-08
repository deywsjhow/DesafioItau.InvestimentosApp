# DesafioItau.InvestimentosApp

•	Explique como aplicar auto-scaling horizontal no serviço 
•	Compare estratégias de balanceamento de carga (round-robin vs latência).


Round-Robin: Distribui as requisições de forma cíclica e sequencial entre os servidores disponíveis - S1 --> S2 --> S3 segue o flixo novamente.

Vantagens: É relativamente simples de de se implementar e entender, 
           garante a distruição das reqs entre os servidores,
           Não necessita de monitoramento de estado dos servidores.

Desvantagens: Não considera tempo de resposta ou carga dos servidores,
              Não verifica a disponibilidade ou a eficacia dos servidores,
              Os servidores precisam ser da mesma capacidade
              

Latência: É baseada em desempenho!

Vantagens: Menor tempo de resposta,
           Valida a disponibilidade e eficiência dos servidores,
           Se adapta a mudanças dinâmicas nas cargas dos servidores

Desvantagens: Mais complexo de se implementar,
              Pode desequilibrar um servidor se ele estiver mais eficaz que os outros,
              Requer atualização frequente.


Testes Mutantes
Explique o conceito de teste mutante e sua importância.

Conceito de TesteMutante:
    Teste Mutante é uma técnica de testes automatizados que cria versões alteradas do código original,
    mudando pequenas partes para verificar se os testes existentes conseguem detectar essas alterações. 

Importância:
    Ele ajuda a garantir a qualidade dos seus testes automatizados, 
    demonstrando se o mesmo é capaz de averiguar erros reais no código.


