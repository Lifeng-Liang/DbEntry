using System;
using System.Collections.Generic;
using Lephone.Data.Definition;
using Lephone.Data;

namespace Lephone
{
    [DbTable("DCS_USERS"), Cacheable]
    public class User : DbObjectModel<User>
    {
        [DbColumn("USER_NAME")]
        public string Name { get; set; }
    }

    [DbTable("REF_ORG_UNIT"), Cacheable]
    public class OrganisationalUnit : DbObjectModel<OrganisationalUnit>
    {
        [HasMany]
        public IList<JobRoleRelation> JobRoleRelations { get; set; }
    }

    [DbTable("HRM_EMPLOYEES"), Cacheable]
    public class Employee : DbObjectModel<Employee>
    {
        [HasOne]
        public EmployeeRoleRelation Rel { get; set; }

        [BelongsTo]
        public Person Person { get; set; }
    }

    [DbTable("DCS_PERSONS")]
    public class Person : DbObjectModel<Person>
    {
        [DbColumn("NAME_LAST")]
        public string LastName { get; set; }

        [HasOne]
        public Employee Emp { get; set; }
    }

    [DbTable("REL_EMP_JOB_ROLE"), Cacheable]
    public class EmployeeRoleRelation : DbObjectModel<EmployeeRoleRelation>
    {
        [DbColumn("UC")]
        public long CreatedBy { get; set; }

        [DbColumn("AF")]
        public bool Active { get; set; }

        [DbColumn("START_DATE")]
        public DateTime? Start { get; set; }

        [BelongsTo]
        public Employee Employee { get; set; }

        [BelongsTo]
        public JobRole Jrr { get; set; }
    }

    [DbTable("REL_JOB_ROLE_ORG_UNIT"), Cacheable]
    public class JobRoleRelation : DbObjectModel<JobRoleRelation>
    {
        [DbColumn("UC")]
        public long CreatedBy { get; set; }

        [DbColumn("AF")]
        public bool Active { get; set; }

        [DbColumn("RELATION_TYPE")]
        public JobRoleRelationType Type { get; set; }

        [BelongsTo]
        public OrganisationalUnit OrganisationalUnit { get; set; }

        [BelongsTo]
        public JobRole JobRole { get; set; }
    }

    [DbTable("HRM_JOB_ROLES"), Cacheable]
    public class JobRole : DbObjectModel<JobRole>
    {
        [DbColumn("UC")]
        public long CreatedBy { get; set; }

        [DbColumn("CODE")]
        public string Code { get; set; }

        [DbColumn("ROLE_NAME")]
        public string Name { get; set; }

        [DbColumn("DESCRIPTION")]
        public string Description { get; set; }

        [HasMany]
        public IList<JobRoleRelation> JobRoleRelations { get; set; }

        [HasMany]
        public IList<EmployeeRoleRelation> EmployeeRoleRelations { get; set; }
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
