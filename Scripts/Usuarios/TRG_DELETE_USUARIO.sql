GO
CREATE TRIGGER TRG_DELETE_USUARIO_INSTEAD 
ON Usuarios
INSTEAD OF DELETE
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Usuario_Id INT, 
	        @Usuario_Codigo NVARCHAR(10)

	-- Cursor para capturar os usuários que seriam deletados
	DECLARE deleted_cursor CURSOR FOR 
	SELECT Id, Codigo FROM DELETED

	OPEN deleted_cursor
	FETCH NEXT FROM deleted_cursor INTO @Usuario_Id, @Usuario_Codigo

	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Remove as referências antes de excluir o usuário
		DELETE FROM Tarefas 
		WHERE UsuarioId = @Usuario_Id
		  AND UsuarioCodigo = @Usuario_Codigo

		DELETE FROM Salarios 
		WHERE UsuarioId = @Usuario_Id
		  AND UsuarioCodigo = @Usuario_Codigo

		DELETE FROM UsuarioCargoDepartamento 
		WHERE UsuarioId = @Usuario_Id
		  AND UsuarioCodigo = @Usuario_Codigo

		DELETE FROM AppUser 
		WHERE UserName = @Usuario_Codigo

		-- Agora, pode excluir o usuário
		DELETE FROM Usuarios 
		WHERE Id = @Usuario_Id
		  AND Codigo = @Usuario_Codigo

		FETCH NEXT FROM deleted_cursor INTO @Usuario_Id, @Usuario_Codigo
	END

	CLOSE deleted_cursor
	DEALLOCATE deleted_cursor
END
