﻿using DAL.DAO;
using HLP.Interfaces;

namespace DAL.Factory
{
    /// <summary>
    /// Classe padrão para os objetos do repository
    /// </summary>
    public class ContextBase<Model, Entity> : Context
        where Model : class
        where Entity : class
    {
        public readonly IObjectModelHelper<Model, Entity> _objectModelHelper;

        public ContextBase(IObjectModelHelper<Model, Entity> objectModelHelper)
        {
            _objectModelHelper = objectModelHelper;
        }

        //protected QRZDatabaseDataContext GetContext()
        //{
        //    var context = _context.GetContext();
        //    return context;
        //}
    }
}