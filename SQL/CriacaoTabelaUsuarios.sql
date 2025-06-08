/*************************************************

	-- Função: Criar tabela/indices
	-- Autor: Jonathan Deyws Santos de Oliveira
	-- Data: 03/06/2025

*************************************************/

SET ANSI_NULLS ON -- Comparação com NULL seguem o padrão ansi. Sempre vai ser false se validar NULL = NULL
GO
SET QUOTED_IDENTIFIER ON -- Permite nome de colunas com espaços ou palavras reservadas (permitir nomes de objetos com aspas duplas)
GO
	
-- DROP TABLE Usuarios
-- CRIAÇÃO DA TABELA Usuarios
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = object_id ('dbo.Usuarios') AND type = 'U')
CREATE TABLE [dbo].[Usuarios] (
   [id]							 [int]						IDENTITY (1, 1) NOT NULL,
   [nome]						 [varchar]			(100)						 NOT NULL,
   [email]						 [varchar]			(150)						 NOT NULL,
   [corretagem_percentual]  [decimal]			(5, 2)		   		 NOT NULL 
) ON [PRIMARY]
GO

-- ADICIONANDO A PK NA TABELA CRIADA
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE name = 'PK_Usuarios' AND parent_object_id = object_id ('dbo.Usuarios') AND  type = 'PK')
ALTER TABLE [dbo].[Usuarios] 
	ADD CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED
	(
		[id]
	) ON [PRIMARY]
GO



-- POPULANDO A TABELA
DROP TABLE IF EXISTS #tmp_Usuarios
GO

SELECT tm.nome, tm.email, tm.corretagem_percentual
	INTO #tmp_Usuarios
		FROM (SELECT 'user1' 'nome', 'user1@teste.com' 'email', 12.1 'corretagem_percentual' UNION ALL
			   SELECT 'user2' 'nome', 'user2@teste.com' 'email', 12.2 'corretagem_percentual' UNION ALL
			   SELECT 'user3' 'nome', 'user3@teste.com' 'email', 12.3 'corretagem_percentual' UNION ALL
			   SELECT 'user4' 'nome', 'user4@teste.com' 'email', 12.4 'corretagem_percentual' UNION ALL
			   SELECT 'user5' 'nome', 'user5@teste.com' 'email', 12.5 'corretagem_percentual') tm


UPDATE xx
	SET xx.nome						   = tm.nome,
		 xx.email					   = tm.email,
		 xx.corretagem_percentual  = tm.corretagem_percentual
	FROM #tmp_Usuarios tm
		WITH (NOLOCK)
	INNER JOIN [dbo].[Usuarios] xx
		WITH (ROWLOCK, INDEX(Ind_Usuarios_01))
		ON tm.email						= xx.email
GO

INSERT INTO [dbo].[Usuarios] (nome, email, corretagem_percentual)
	SELECT tm.nome, tm.email, tm.corretagem_percentual
		FROM #tmp_Usuarios tm
			WITH (NOLOCK)
		WHERE NOT EXISTS (SELECT TOP 1 1
									FROM [dbo].[Usuarios] xx
										WITH (NOLOCK, INDEX(Ind_Usuarios_01))
									WHERE xx.email = tm.email)
GO

-- ADICIONANDO INDICE COMPOSTO
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.indexes WHERE name = 'Ind_Usuarios_01' AND object_id = object_id ('dbo.Usuarios'))
CREATE NONCLUSTERED INDEX [Ind_Usuarios_01]
	ON [dbo].[Usuarios] ([email], [nome]) ON [PRIMARY]
GO



