/** 
		1.	Proponha e justifique um ou mais �ndices para essa consulta.
		R: Para esse consulta, ser� utilizada mais de 1 tabela: Opera��es, usuario e Ativos.

			Para a tabela de opera��es, escolhi criar e utilizar o indice Ind_Operacoes_01: Com ele, consulto pelo id_usuario e id_ativo.
			Na consulta em especifico, utilizo somente a coluna1 j� que ela � a primeira do indice e permanece perform�tica.

			Para a tabela Ativos, utilizo a PK como indice. � perform�tica e atende a consulta.

			Para a de usu�rios, vou pela PK tamb�m por ser mais perform�tica.

			2.	Escreva o SQL da consulta otimizada.
			R: c�digo abaixo

			3.	Crie a estrutura para atualiza��o da Posi��o, com base na cota��o.
			R: C�digo abaixo. Ao inv�s de criar uma procedure, decidi fazer somente uma query de update, fazendo oq o passo pede.

**/


--1. O sistema precisa consultar rapidamente todas as opera��es de um usu�rio em determinado ativo nos �ltimos 30 dias.
SELECT us.nome 'NomUser', av.nome 'NomAtivo',
       sum (op.quantidade * (op.preco_unitario + op.Corretagem)) / nullif (sum (op.quantidade), 0) 'PrecoMedio'
	FROM [dbo].[Operacoes] op 
	    WITH (NOLOCK, INDEX(Ind_Operacoes_01))
	INNER JOIN [dbo].[Usuarios] us
	    WITH (NOLOCK INDEX (1)) 
	    ON us.id = op.id_usuario
	INNER JOIN [dbo].[Ativos] av
	    WITH (NOLOCK, INDEX (1)) 
	    ON av.id = op.id_ativo
	WHERE op.id_usuario = @Id_usuario
	  AND op.id_ativo = @Id_Ativo 
	  AND op.tipo_operacao = 'c'
	  AND op.data_compra >= dateadd (day, -30, getdate ())
	  GROUP BY us.nome, av.nome
	  ORDER BY av.nome


-- 3.	Crie a estrutura para atualiza��o da Posi��o, com base na cota��o.

UPDATE ps
SET pl = (cs.preco_unitario - ps.preco_medio) * ps.quantidade
FROM [dbo].[Posicoes] ps
    WITH (UPDLOCK, INDEX(1))
INNER JOIN [dbo].[Usuarios] us
    WITH (NOLOCK, INDEX(1))
    ON us.id = ps.id_usuario
CROSS APPLY (
    SELECT TOP 1 cs.preco_unitario
    FROM [dbo].[Cotacoes] cs
        WITH (NOLOCK, INDEX(1))
    WHERE cs.id_ativo = ps.id_ativo
    ORDER BY cs.data_hora DESC
) cs;
