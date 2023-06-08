namespace NTF.Interfaces
{
    /// <summary>
    /// Interface que define um nível de gravidade para uma entidade ou evento. <br></br>
    /// Fornece a propriedade de descrição e o método de representação em string.
    /// </summary>
    public interface ILevel
    {
        /// <summary>
        /// Obtém a descrição do nível de gravidade.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Retorna a representação em string do nível de gravidade.
        /// </summary>
        /// <returns>A descrição do nível de gravidade.</returns>
        string ToString();
    }
}
