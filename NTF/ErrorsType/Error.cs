using NTF.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NTF.ErrorsType
{
    /// <summary>
    /// Classe que representa uma coleção de erros em um contexto de notificações.<br></br>
    /// Herda da classe base Notification e fornece propriedades e métodos para manipular erros e obter informações sobre sua presença e tipo.
    /// </summary>
    public class Error : Notification
    {
        /// <summary>
        /// Obtém uma lista de descrições de erros críticos.
        /// </summary>
        public IList<ErrorDescription> Errors
        {
            get
            {
                return GetList().Cast<ErrorDescription>().Where(x => x.GetLevel() is Critical).ToList();
            }
        }

        /// <summary>
        /// Obtém uma lista de descrições de avisos.
        /// </summary>
        public IList<ErrorDescription> Warnings
        {
            get
            {
                return GetList().Cast<ErrorDescription>().Where(x => x.GetLevel() is Warning).ToList();
            }
        }

        /// <summary>
        /// Obtém uma lista de descrições de informações.
        /// </summary>
        public IList<ErrorDescription> Informations
        {
            get
            {
                return GetList().Cast<ErrorDescription>().Where(x => x.GetLevel() is Information).ToList();
            }
        }

        /// <summary>
        /// Obtém um valor que indica se existem erros críticos.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return GetList().Cast<ErrorDescription>().Any(x => x.GetLevel() is Critical);
            }
        }

        /// <summary>
        /// Obtém um valor que indica se existem avisos.
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                return GetList().Cast<ErrorDescription>().Any(x => x.GetLevel() is Warning);
            }
        }

        /// <summary>
        /// Obtém um valor que indica se existem informações.
        /// </summary>
        public bool HasInformations
        {
            get
            {
                return GetList().Cast<ErrorDescription>().Any(x => x.GetLevel() is Information);
            }
        }

    }
}
