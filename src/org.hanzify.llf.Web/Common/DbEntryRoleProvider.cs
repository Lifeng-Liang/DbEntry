
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using Lephone.Data;

namespace Lephone.Web.Common
{
    public class DbEntryRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            WhereCondition c = null;
            foreach (string s in roleNames)
            {
                c |= CK.K["Name"] == s;
            }
            WhereCondition cu = null;
            foreach (string s in usernames)
            {
                cu |= CK.K["UserName"] == s;
            }
            DbEntry.UsingExistedTransaction(delegate()
            {
                List<DbEntryMembershipUser> ls = DbEntryMembershipUser.Find(cu);
                foreach (DbEntryRole r in DbEntryRole.Find(c))
                {
                    foreach (DbEntryMembershipUser u in ls)
                    {
                        r.Users.Add(u);
                    }
                    r.Save();
                }
            });
        }

        public override string ApplicationName
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override void CreateRole(string roleName)
        {
            DbEntryRole.New().Init(roleName).Save();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            DbEntryRole r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            if (r != null)
            {
                if (throwOnPopulatedRole && r.Users.Count>0)
                {
                    throw new DbEntryException("This role has members.");
                }
                r.Delete();
                return true;
            }
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            DbEntryRole r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            if (r != null)
            {
                List<string> ls = new List<string>();
                foreach (DbEntryMembershipUser u in r.Users)
                {
                    if (u.UserName.IndexOf(usernameToMatch) >= 0)
                    {
                        ls.Add(u.UserName);
                    }
                }
                return ls.ToArray();
            }
            return null;
        }

        public override string[] GetAllRoles()
        {
            List<string> ls = new List<string>();
            foreach (DbEntryRole r in DbEntryRole.FindAll())
            {
                ls.Add(r.Name);
            }
            return ls.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            List<string> ls = new List<string>();
            foreach (DbEntryRole r in u.Roles)
            {
                ls.Add(r.Name);
            }
            return ls.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            DbEntryRole r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            List<string> ls = new List<string>();
            foreach (DbEntryMembershipUser u in r.Users)
            {
                ls.Add(u.UserName);
            }
            return ls.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            DbEntryMembershipUser u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            foreach (DbEntryRole r in u.Roles)
            {
                if (r.Name == roleName)
                {
                    return true;
                }
            }
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            WhereCondition c = null;
            foreach (string s in roleNames)
            {
                c |= CK.K["Name"] == s;
            }
            List<string> uns = new List<string>(usernames);
            DbEntry.UsingExistedTransaction(delegate()
            {
                foreach (DbEntryRole r in DbEntryRole.Find(c))
                {
                    for (int i = r.Users.Count - 1; i >=0; i--)
                    {
                        if (uns.Contains(r.Users[i].UserName))
                        {
                            r.Users.RemoveAt(i);
                        }
                    }
                    r.Save();
                }
            });
        }

        public override bool RoleExists(string roleName)
        {
            DbEntryRole r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            return (r != null);
        }
    }
}
