/*************************************************

	-- Função: Criar tabela/indices
	-- Autor: Jonathan Deyws Santos de Oliveira
	-- Data: 03/06/2025

*************************************************/

SET ANSI_NULLS ON -- Comparação com NULL seguem o padrão ansi. Sempre vai ser false se validar NULL = NULL
GO
SET QUOTED_IDENTIFIER ON -- Permite nome de colunas com espaços ou palavras reservadas (permitir nomes de objetos com aspas duplas)
GO
	


-- CRIAÇÃO DA TABELA Usuarios
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = object_id ('dbo.Ativos') AND type = 'U')
CREATE TABLE [dbo].[Ativos] (
   [id]						 [int]						IDENTITY (1, 1) NOT NULL,
	[codigo]					 [varchar]			(10)						 NOT NULL,
   [nome]					 [varchar]			(100)						 NOT NULL,
) ON [PRIMARY]
GO

-- ADICIONANDO A PK NA TABELA CRIADA
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'PK_ativos' AND parent_object_id = object_id ('dbo.Ativos') AND  type = 'PK')
ALTER TABLE [dbo].[Ativos] 
	ADD CONSTRAINT [PK_ativos] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.indexes WHERE name = 'Ind_Ativos_01' AND object_id = object_id ('dbo.Ativos'))
CREATE NONCLUSTERED INDEX [Ind_Ativos_01]
	ON [dbo].[Ativos] ([codigo]) ON [PRIMARY]
GO



DROP TABLE IF EXISTS #tmp_Ativos
GO

SELECT tm.Codigo, tm.Nome
	INTO #tmp_Ativos
	FROM (
		SELECT 'PETR4'  'Codigo', 'Petrobras PN'			'Nome'	UNION ALL
		SELECT 'VALE3'  'Codigo', 'Vale ON'					'Nome'	UNION ALL
		SELECT 'ITUB4'  'Codigo', 'Itaú Unibanco PN'		'Nome'	UNION ALL
		SELECT 'ABEV3'  'Codigo', 'Ambev ON'				'Nome'	UNION ALL
		SELECT 'BBAS3'  'Codigo', 'Banco do Brasil ON'	'Nome'	
	) tm
GO


UPDATE xx
	SET xx.Codigo = tm.Codigo,
	    xx.Nome = tm.Nome
	FROM #tmp_Ativos tm
		WITH (NOLOCK)
	INNER JOIN [dbo].[Ativos] xx
		WITH (ROWLOCK, INDEX(Ind_Ativos_01))
		ON xx.Codigo = tm.Codigo
GO


INSERT INTO [dbo].[Ativos] (Codigo, Nome)
	SELECT tm.Codigo, tm.Nome
		FROM #tmp_Ativos tm
			WITH (NOLOCK)
		WHERE NOT EXISTS (SELECT TOP 1 1
									FROM [dbo].[Ativos] xx
										WITH (NOLOCK, INDEX(Ind_Ativos_01))
									WHERE xx.Codigo = tm.Codigo)
GO






