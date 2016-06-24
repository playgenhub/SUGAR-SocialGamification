﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.Data.EntityFramework.Controllers
{
	public class AccountController : DbController
	{
		public AccountController(string nameOrConnectionString) 
			: base(nameOrConnectionString)
		{
		}

		public Account Create(Account account)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				if (context.Entry(account.User).State == EntityState.Detached)
				{
					context.Users.Attach(account.User);
				}

				context.Accounts.Add(account);
				SaveChanges(context);

				return account;
			}
		}

		public IEnumerable<Account> Get(string[] names)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var accounts = context.Accounts
					.Where(a => names.Contains(a.Name))
					.Include(a => a.User);

				return accounts.ToList();
			}
		}

		public void Delete(int[] id)
		{
			using (var context = new SGAContext(NameOrConnectionString))
			{
				SetLog(context);

				var accounts = context.Accounts.Where(g => id.Contains(g.Id));
				context.Accounts.RemoveRange(accounts);
				SaveChanges(context);
			}
		}
	}
}
