﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PlayGen.SGA.DataAccess;
using PlayGen.SGA.DataController.Exceptions;
using PlayGen.SGA.DataModel;

namespace PlayGen.SGA.DataController
{
    public class GameDbController : DbController
    {
        public GameDbController(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public IEnumerable<Game> Get()
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var games = context.Games.ToList();

                return games;
            }
        }

        /// <summary>
        /// Retrieve game multiple records by name from the database
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public IEnumerable<Game> Get(string[] names)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var games = context.Games.Where(g => names.Contains(g.Name)).ToList();

                return games;
            }
        }

        /// <summary>
        /// Create a new game record in the database.
        /// </summary>
        /// <param name="newGame"></param>
        /// <returns></returns>
        public void Create(Game game)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);
                
                context.Games.Add(game);
                SaveChanges(context);
            }
        }

        /// <summary>
        /// Delete a game record from the database.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int[] id)
        {
            using (var context = new SGAContext(NameOrConnectionString))
            {
                SetLog(context);

                var games = context.Games.Where(g => id.Contains(g.Id)).ToList();

                context.Games.RemoveRange(games);
                SaveChanges(context);
            }
        }
    }
}
