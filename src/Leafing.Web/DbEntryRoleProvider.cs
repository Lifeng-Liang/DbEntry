using System;
using System.Collections.Generic;
using System.Web.Security;
using Leafing.Data;

namespace Leafing.Web
{
    public class DbEntryRoleProvider : RoleProvider
    {
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            Condition c = null;
            foreach (string s in roleNames)
            {
                c |= CK.K["Name"] == s;
            }
            Condition cu = null;
            foreach (string s in usernames)
            {
                cu |= CK.K["UserName"] == s;
            }
            DbEntry.UsingTransaction(delegate
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
            new DbEntryRole{Name = roleName}.Save();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            if (r != null)
            {
                if (throwOnPopulatedRole && r.Users.Count>0)
                {
                    throw new DataException("This role has members.");
                }
                r.Delete();
                return true;
            }
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            if (r != null)
            {
                var ls = new List<string>();
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
            var ls = new List<string>();
            foreach (DbEntryRole r in DbEntryRole.Find(Condition.Empty))
            {
                ls.Add(r.Name);
            }
            return ls.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
            var ls = new List<string>();
            foreach (DbEntryRole r in u.Roles)
            {
                ls.Add(r.Name);
            }
            return ls.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            var ls = new List<string>();
            foreach (DbEntryMembershipUser u in r.Users)
            {
                ls.Add(u.UserName);
            }
            return ls.ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var u = DbEntryMembershipUser.FindOne(CK.K["UserName"] == username);
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
            Condition c = null;
            foreach (string s in roleNames)
            {
                c |= CK.K["Name"] == s;
            }
            var uns = new List<string>(usernames);
            DbEntry.UsingTransaction(delegate
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
            var r = DbEntryRole.FindOne(CK.K["Name"] == roleName);
            return (r != null);
        }
    }
}
