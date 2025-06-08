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
IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_cotacoes_ativos' AND parent_object_id = object_id ('dbo.Cotacoes') AND type = 'F')
ALTER TABLE [dbo].[Cotacoes] DROP CONSTRAINT [fk_cotacoes_ativos]
GO

-- DROP TABLE Cotacoes	
-- CRIAÇÃO DA TABELA Cotacoes
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = object_id ('dbo.Cotacoes') AND type = 'U')
CREATE TABLE [dbo].[Cotacoes] (
   [id]						 [bigint]						IDENTITY (1, 1) NOT NULL,
   [id_ativo]				 [int]												 NOT NULL,
   [preco_unitario]		 [decimal]			(15, 4)		   			 NOT NULL,
	[data_hora]				 [datetime2]										 NOT NULL
) ON [PRIMARY]	
GO


-- ADICIONANDO A PK NA TABELA CRIADA
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'PK_Cotacoes' AND parent_object_id = object_id ('dbo.Cotacoes') AND  type = 'PK')
ALTER TABLE [dbo].[Cotacoes] 
	ADD CONSTRAINT [PK_Cotacoes] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO


-- ADICIONANDO INDICE COMPOSTO
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.indexes WHERE name = 'Ind_Cotacoes_01' AND object_id = object_id ('dbo.Cotacoes'))
CREATE NONCLUSTERED INDEX [Ind_Cotacoes_01]
	ON [dbo].[Cotacoes] ([id_ativo], [data_hora]) ON [PRIMARY]
GO


--DROP TABLE IF EXISTS #tmp_Cotacoes
--GO


--SELECT tm.id_ativo, tm.preco_unitario, tm.data_hora
--	INTO #tmp_Cotacoes
--	FROM (
--			 SELECT 1 'id_ativo', 12.50 'preco_unitario', getdate () 'data_hora' UNION ALL
--			 SELECT 2 'id_ativo', 12.60 'preco_unitario', getdate () 'data_hora' UNION ALL
--			 SELECT 3 'id_ativo', 12.70 'preco_unitario', getdate () 'data_hora' UNION ALL
--			 SELECT 4 'id_ativo', 12.80 'preco_unitario', getdate () 'data_hora' UNION ALL
--			 SELECT 5 'id_ativo', 12.90 'preco_unitario', getdate () 'data_hora'
--	) tm
--GO


--UPDATE xx
--	SET xx.id_ativo		  = tm.id_ativo,
--	    xx.preco_unitario  = tm.preco_unitario,
--	    xx.data_hora		  = tm.data_hora
--	FROM #tmp_Cotacoes tm
--		WITH (NOLOCK)
--	INNER JOIN [dbo].[Cotacoes] xx
--		WITH (ROWLOCK, INDEX(Ind_Cotacoes_01))
--		ON xx.id_ativo = tm.id_ativo
--GO


--INSERT INTO [dbo].[Cotacoes] (id_ativo, preco_unitario, data_hora)
--	SELECT tm.id_ativo, tm.preco_unitario, tm.data_hora
--		FROM #tmp_Cotacoes tm
--			WITH (NOLOCK)
--		WHERE NOT EXISTS (SELECT TOP 1 1
--									FROM [dbo].[Cotacoes] xx
--										WITH (NOLOCK, INDEX(Ind_Cotacoes_01))
--									WHERE xx.id_ativo = tm.id_ativo)
--GO


IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_cotacoes_ativos' AND parent_object_id = object_id ('dbo.Cotacoes') AND type = 'F')
ALTER TABLE [dbo].[Cotacoes] WITH NOCHECK ADD
   CONSTRAINT [fk_cotacoes_ativos] FOREIGN KEY 
	(
		[id_ativo]
	) REFERENCES [dbo].[Ativos]( 
		[id]
	)
GO

