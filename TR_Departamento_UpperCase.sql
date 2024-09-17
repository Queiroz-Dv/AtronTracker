CREATE TRIGGER TR_Departamento_UpperCase
ON Departamentos
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Atualiza a tabela 'Departamento' ajustando as colunas 'Codigo' e 'Descricao' para caixa alta
    UPDATE Departamento
    SET Codigo = UPPER(i.Codigo),
        Descricao = UPPER(i.Descricao)
    FROM inserted i
    WHERE Departamento.Id = i.Id;
END;
GO