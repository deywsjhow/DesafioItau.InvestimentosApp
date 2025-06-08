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
IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_operacoes_usuarios' AND parent_object_id = object_id ('dbo.Operacoes') AND type = 'F')
ALTER TABLE [dbo].[Operacoes] DROP CONSTRAINT [fk_operacoes_usuarios]
GO

IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_operacoes_ativos' AND parent_object_id = object_id ('dbo.Operacoes') AND type = 'F')
ALTER TABLE [dbo].[Operacoes] DROP CONSTRAINT [fk_operacoes_ativos]
GO
	
-- DROP TABLE Operacoes
-- CRIAÇÃO DA TABELA Operacoes
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = object_id ('dbo.Operacoes') AND type = 'U')
CREATE TABLE [dbo].[Operacoes] (
   [id]						 [int]						IDENTITY (1, 1) NOT NULL,
   [id_usuario]			 [int]											 NOT NULL,
   [id_ativo]				 [int]											 NOT NULL,
   [quantidade]			 [int]											 NOT NULL,
   [preco_unitario]		 [decimal]			(15, 4)		   		 NOT NULL,
   [tipo_operacao]		 [char]				(1)						 NOT NULL,  -- Compra = 'c', Venda = 'v'
   [corretagem]			 [decimal]			(10, 2)		   		 NOT NULL, 
	[data_compra]			 [datetime2]									 NOT NULL
) ON [PRIMARY]	
GO

-- ADICIONANDO A PK NA TABELA CRIADA
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'PK_Usuarios' AND parent_object_id = object_id ('dbo.Operacoes') AND  type = 'PK')
ALTER TABLE [dbo].[Operacoes] 
	ADD CONSTRAINT [PK_Operacoes] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO



-- ADICIONANDO INDICE COMPOSTO
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.indexes WHERE name = 'Ind_Usuarios_01' AND object_id = object_id ('dbo.Operacoes'))
CREATE NONCLUSTERED INDEX [Ind_Operacoes_01]
	ON [dbo].[Operacoes] ([id_usuario], [id_ativo]) ON [PRIMARY]
GO




DROP TABLE IF EXISTS #tmp_Operacoes
GO


SELECT tm.id_usuario, tm.id_ativo, tm.quantidade, tm.preco_unitario, tm.tipo_operacao, tm.corretagem, tm.data_compra
	INTO #tmp_Operacoes
	FROM (
			 SELECT 1 'id_usuario', 1 'id_ativo', 90 'quantidade', 12.50 'preco_unitario', 'c' 'tipo_operacao', 5.00 'corretagem', getdate () 'data_compra' UNION ALL
			 SELECT 1 'id_usuario', 2 'id_ativo', 50 'quantidade', 22.75 'preco_unitario', 'c' 'tipo_operacao', 5.00 'corretagem', getdate () 'data_compra' UNION ALL
			 SELECT 2 'id_usuario', 5 'id_ativo', 30 'quantidade', 13.00 'preco_unitario', 'v' 'tipo_operacao', 4.50 'corretagem', getdate () 'data_compra' UNION ALL
			 SELECT 3 'id_usuario', 3 'id_ativo', 70 'quantidade', 15.90 'preco_unitario', 'c' 'tipo_operacao', 5.50 'corretagem', getdate () 'data_compra' UNION ALL
			 SELECT 4 'id_usuario', 4 'id_ativo', 60 'quantidade', 18.40 'preco_unitario', 'v' 'tipo_operacao', 6.00 'corretagem', getdate () 'data_compra'
	) tm
GO


UPDATE xx
	SET xx.id_usuario	      = tm.id_usuario,
	    xx.id_ativo		   = tm.id_ativo,
	    xx.quantidade	      = tm.quantidade,
	    xx.preco_unitario   = tm.preco_unitario,
	    xx.tipo_operacao    = tm.tipo_operacao,
	    xx.corretagem	      = tm.corretagem,
	    xx.data_compra	   = tm.data_compra
	FROM #tmp_Operacoes tm
		WITH (NOLOCK)
	INNER JOIN [dbo].[Operacoes] xx
		WITH (ROWLOCK)
		ON xx.id_usuario     = tm.id_usuario 
	  AND xx.id_ativo		   = tm.id_ativo 
	  AND xx.data_compra    = tm.data_compra
GO


INSERT INTO [dbo].[Operacoes] (id_usuario, id_ativo, quantidade, preco_unitario, tipo_operacao, corretagem, data_compra)
	SELECT tm.id_usuario, tm.id_ativo, tm.quantidade, tm.preco_unitario, tm.tipo_operacao, tm.corretagem, tm.data_compra
		FROM #tmp_Operacoes tm
			WITH (NOLOCK)
		WHERE NOT EXISTS (SELECT TOP 1 1
									FROM [dbo].[Operacoes] xx
										WITH (NOLOCK)
									WHERE xx.id_usuario  = tm.id_usuario 
								     AND xx.id_ativo    = tm.id_ativo 
									  AND xx.data_compra = tm.data_compra)
GO



-- ADICIONANDO FKS NA TABELA CRIADA
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_operacoes_usuarios' AND parent_object_id = object_id ('dbo.Operacoes') AND type = 'F')
ALTER TABLE [dbo].[Operacoes] WITH NOCHECK ADD
   CONSTRAINT [fk_operacoes_usuarios] FOREIGN KEY 
	(
		[id_usuario]
	) REFERENCES [dbo].[Usuarios]( 
		[id]
	)
GO


IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'fk_operacoes_ativos' AND parent_object_id = object_id ('dbo.Operacoes') AND type = 'F')
ALTER TABLE [dbo].[Operacoes] WITH NOCHECK ADD
   CONSTRAINT [fk_operacoes_ativos] FOREIGN KEY 
	(
		[id_ativo]
	) REFERENCES [dbo].[Ativos]( 
		[id]
	)
GO


