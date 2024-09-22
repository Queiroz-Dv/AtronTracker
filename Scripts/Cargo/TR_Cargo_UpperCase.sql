CREATE TRIGGER TR_Cargo_UpperCase
ON Cargos
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Atualiza a tabela 'Cargo' ajustando as colunas 'Codigo' e 'Descricao' para caixa alta
    UPDATE Cargo
    SET Codigo = UPPER(i.Codigo),
        Descricao = UPPER(i.Descricao)
    FROM inserted i
    WHERE Cargo.Id = i.Id;
END;
GO