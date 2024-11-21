CREATE TRIGGER TR_Usuarios_UpperCase
ON Usuarios
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Atualiza a tabela 'Usuários' ajustando a coluna 'Codigo' para caixa alta
    UPDATE Usuarios
    SET Codigo = UPPER(i.Codigo)
    FROM inserted i
    WHERE Usuarios.Id = i.Id;
END;
GO