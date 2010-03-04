using System;
using System.Collections.Generic;
using Lephone.Data.Definition;
using Lephone.Data;
using Lephone.Data.Common;

namespace Lephone
{
    [DbTable("DCS_USERS"), Cacheable]
    public abstract class User : DbObjectModel<User>
    {
        [DbColumn("USER_NAME")]
        public abstract string Name { get; set; }
    }

    [DbTable("REF_ORG_UNIT"), Cacheable]
    public abstract class OrganisationalUnit : DbObjectModel<OrganisationalUnit>
    {
        [HasMany]
        public abstract IList<JobRoleRelation> JobRoleRelations { get; set; }
    }

    [DbTable("HRM_EMPLOYEES"), Cacheable]
    public abstract class Employee : DbObjectModel<Employee>
    {
        [HasOne]
        public abstract EmployeeRoleRelation Rel { get; set; }

        [BelongsTo]
        public abstract Person Person { get; set; }
    }

    [DbTable("DCS_PERSONS")]
    public abstract class Person : DbObjectModel<Person>
    {
        [DbColumn("NAME_LAST")]
        public abstract string LastName { get; set; }

        [HasOne]
        public abstract Employee emp { get; set; }
    }

    [DbTable("REL_EMP_JOB_ROLE"), Cacheable]
    public abstract class EmployeeRoleRelation : DbObjectModel<EmployeeRoleRelation>
    {
        [DbColumn("UC")]
        public abstract long CreatedBy { get; set; }

        [DbColumn("AF")]
        public abstract bool Active { get; set; }

        [DbColumn("START_DATE")]
        public abstract DateTime? Start { get; set; }

        [BelongsTo]
        public abstract Employee Employee { get; set; }

        [BelongsTo]
        public abstract JobRole jrr { get; set; }
    }

    [DbTable("REL_JOB_ROLE_ORG_UNIT"), Cacheable]
    public abstract class JobRoleRelation : DbObjectModel<JobRoleRelation>
    {
        [DbColumn("UC")]
        public abstract long CreatedBy { get; set; }

        [DbColumn("AF")]
        public abstract bool Active { get; set; }

        [DbColumn("RELATION_TYPE")]
        public abstract JobRoleRelationType Type { get; set; }

        [BelongsTo]
        public abstract OrganisationalUnit OrganisationalUnit { get; set; }

        [BelongsTo]
        public abstract JobRole JobRole { get; set; }
    }

    [DbTable("HRM_JOB_ROLES"), Cacheable]
    public abstract class JobRole : DbObjectModel<JobRole>
    {
        [DbColumn("UC")]
        public abstract long CreatedBy { get; set; }

        [DbColumn("CODE")]
        public abstract string Code { get; set; }

        [DbColumn("ROLE_NAME")]
        public abstract string Name { get; set; }

        [DbColumn("DESCRIPTION")]
        public abstract string Description { get; set; }

        [HasMany]
        public abstract IList<JobRoleRelation> JobRoleRelations { get; set; }

        [HasMany]
        public abstract IList<EmployeeRoleRelation> EmployeeRoleRelations { get; set; }
    }

    public enum JobRoleRelationType
    {
        Manager,
    }

    public class Program
    {
        static void Main()
        {
            Process(DbContext.GetInstance("3"), "-->> Using SQLite : ");

            Console.WriteLine("-- The End --");
            Console.ReadLine();
        }

        private static void Process(DbContext ds, string s)
        {
            Console.WriteLine("-->>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n{0}", s);
            ds.Create(typeof(User));
            ds.Create(typeof(OrganisationalUnit));
            ds.Create(typeof(Employee));
            ds.Create(typeof(Person));
            ds.Create(typeof(EmployeeRoleRelation));
            ds.Create(typeof(JobRoleRelation));
            ds.Create(typeof(JobRole));
        }
    }
}
