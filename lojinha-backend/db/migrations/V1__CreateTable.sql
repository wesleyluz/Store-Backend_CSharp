USE Loja
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE Produtos(

    [Id] INT IDENTITY(1, 1) NOT NULL,
    [nome] NVARCHAR(500) NULL,
    [descricao]  NVARCHAR(500) NULL,
    [preco] DECIMAL(10,2) NOT NULL,
    [descricaoPreco] NVARCHAR(500) NULL,
    [quantidadeEstoque] INT NOT NULL,
);
