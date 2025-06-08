/*************************************************

	-- Função: Criar tabela/indices
	-- Autor: Jonathan Deyws Santos de Oliveira
	-- Data: 03/06/2025

*************************************************/

SET ANSI_NULLS ON -- Comparação com NULL seguem o padrão ansi. Sempre vai ser false se validar NULL = NULL
GO
SET QUOTED_IDENTIFIER ON -- Permite nome de colunas com espaços ou palavras reservadas (permitir nomes de objetos com aspas duplas)
GO

-- DROP FK SE EXISTIR
IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_posicoes_usuarios' AND parent_object_id = object_id ('dbo.Posicoes') AND type = 'F')
ALTER TABLE [dbo].[Posicoes] DROP CONSTRAINT [fk_posicoes_usuarios]
GO


IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_posicoes_ativos' AND parent_object_id = object_id ('dbo.Posicoes') AND type = 'F')
ALTER TABLE [dbo].[Posicoes] DROP CONSTRAINT [fk_posicoes_ativos]
GO


-- Drop Table Posicoes	
-- CRIAÇÃO DA TABELA posicoes
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = object_id ('dbo.Posicoes') AND type = 'U')
CREATE TABLE [dbo].[Posicoes] (
   [id]						 [int]							IDENTITY (1, 1) NOT NULL,
   [id_usuario]			 [int]												 NOT NULL,
   [id_ativo]				 [int]												 NOT NULL,
   [quantidade]			 [int]												 NOT NULL,
   [preco_medio]	 	    [decimal]			(15, 4)		   			 NOT NULL,
   [pl]	 				    [decimal]			(15, 2)		   			 NOT NULL
) ON [PRIMARY]	
GO


-- ADICIONANDO A PK NA TABELA CRIADA
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'PK_Posicoes' AND parent_object_id = object_id ('dbo.Posicoes') AND  type = 'PK')
ALTER TABLE [dbo].[Posicoes] 
	ADD CONSTRAINT [PK_Posicoes] PRIMARY KEY CLUSTERED
	(
		[id_usuario], [id_ativo]
	) ON [PRIMARY]
GO


DROP TABLE IF EXISTS #tmp_Posicoes
GO


SELECT tm.id_usuario, tm.id_ativo, tm.quantidade, tm.preco_medio, tm.pl
	INTO #tmp_Posicoes
	FROM (
			 SELECT 1 'id_usuario', 1 'id_ativo', 90 'quantidade', 12.50 'preco_medio', 90.00 'pl'	UNION ALL
			 SELECT 1 'id_usuario', 2 'id_ativo', 50 'quantidade', 22.00 'preco_medio', 75.00 'pl' UNION ALL
			 SELECT 2 'id_usuario', 1 'id_ativo', 30 'quantidade', 13.20 'preco_medio', 25.00 'pl' UNION ALL
			 SELECT 3 'id_usuario', 3 'id_ativo', 70 'quantidade', 15.50 'preco_medio', 97.00 'pl'	UNION ALL
			 SELECT 4 'id_usuario', 4 'id_ativo', 60 'quantidade', 18.70 'preco_medio', 80.00 'pl'
	) tm
GO


UPDATE xx
	SET xx.id_usuario	  = tm.id_usuario,
	    xx.id_ativo	  = tm.id_ativo,
	    xx.quantidade	  = tm.quantidade,
	    xx.preco_medio  = tm.preco_medio,
	    xx.pl			  = tm.pl
	FROM #tmp_Posicoes tm
		WITH (NOLOCK)
	INNER JOIN [dbo].[Posicoes] xx
		WITH (ROWLOCK)
		ON xx.id_usuario = tm.id_usuario 
	  AND xx.id_ativo	  = tm.id_ativo
GO


INSERT INTO [dbo].[Posicoes] (id_usuario, id_ativo, quantidade, preco_medio, pl)
	SELECT tm.id_usuario, tm.id_ativo, tm.quantidade, tm.preco_medio, tm.pl
		FROM #tmp_Posicoes tm
			WITH (NOLOCK)
		WHERE NOT EXISTS (SELECT TOP 1 1
									FROM [dbo].[Posicoes] xx
										WITH (NOLOCK)
									WHERE xx.id_usuario = tm.id_usuario 
									  AND xx.id_ativo = tm.id_ativo)
GO



IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_posicoes_usuarios' AND parent_object_id = object_id ('dbo.Posicoes') AND type = 'F')
ALTER TABLE [dbo].[Posicoes] WITH NOCHECK ADD
   CONSTRAINT [fk_posicoes_usuarios] FOREIGN KEY 
	(
		[id_usuario]
	) REFERENCES [dbo].[Usuarios]( 
		[id]
	)
GO



IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_posicoes_ativos' AND parent_object_id = object_id ('dbo.Posicoes') AND type = 'F')
ALTER TABLE [dbo].[Posicoes] WITH NOCHECK ADD
   CONSTRAINT [fk_posicoes_ativos] FOREIGN KEY 
	(
		[id_ativo]
	) REFERENCES [dbo].[Ativos]( 
		[id]
	)
GO
