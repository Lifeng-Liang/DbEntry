using System;
using System.Collections.Generic;
using Lephone.Data.Caching;
using Lephone.Data.Common;
using Lephone.Data.Definition;
using Lephone.Linq;
using Lephone.UnitTest.util;
using Lephone.Util;
using NUnit.Framework;

namespace Lephone.UnitTest.Data
{
    [TestFixture]
    public class CacheTest : DataTestBase
    {
        #region models

        [DbTable("DCS_USERS"), Cacheable]
        public abstract class User : LinqObjectModel<User>
        {
            [DbColumn("USER_NAME")]
            public abstract string Name { get; set; }
        }

        [DbTable("REF_ORG_UNIT"), Cacheable]
        public abstract class OrganisationalUnit : LinqObjectModel<OrganisationalUnit>
        {
            [HasMany]
            public abstract IList<JobRoleRelation> JobRoleRelations { get; set; }
        }

        [DbTable("HRM_EMPLOYEES"), Cacheable]
        public abstract class Employee : LinqObjectModel<Employee>
        {
            [HasOne]
            public abstract EmployeeRoleRelation Rel { get; set; }

            [BelongsTo]
            public abstract Person Person { get; set; }
        }

        [DbTable("DCS_PERSONS")]
        public abstract class Person : LinqObjectModel<Person>
        {
            [DbColumn("NAME_LAST")]
            public abstract string LastName { get; set; }

            [HasOne]
            public abstract Employee emp { get; set; }
        }

        [DbTable("REL_EMP_JOB_ROLE"), Cacheable]
        public abstract class EmployeeRoleRelation : LinqObjectModel<EmployeeRoleRelation>
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
        public abstract class JobRoleRelation : LinqObjectModel<JobRoleRelation>
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
        public abstract class JobRole : LinqObjectModel<JobRole>
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

        #endregion

        [Test]
        public void Test1()
        {
            var now = (MockMiscProvider)MiscProvider.Instance;
            now.SetNow(new DateTime(2007, 11, 4, 15, 23, 43));
            var c = ClassHelper.CreateInstance<StaticHashCacheProvider>();

            var p = new SinglePerson {Id = 15, Name = "tom"};

            string key = KeyGenerator.Instance.GetKey(p.GetType(), p.Id);
            c[key] = ObjectInfo.CloneObject(p);

            var act = (SinglePerson)c[key];

            Assert.IsNotNull(act);
            Assert.AreEqual(15, act.Id);
            Assert.AreEqual("tom", act.Name);

            p.Name = "jerry";
            c[key] = ObjectInfo.CloneObject(p);

            act = (SinglePerson)c[key];
            p.Name = "mike"; // By using ObjectInfo.CloneObject, it doesn't change cached object.

            Assert.IsNotNull(act);
            Assert.AreEqual(15, act.Id);
            Assert.AreEqual("jerry", act.Name);

            now.SetNow(new DateTime(2007, 11, 4, 15, 34, 43));

            act = (SinglePerson)c[key];

            Assert.IsNull(act);
        }

        /*
        Hi Lifeng, 

        here¡äs my testing code (all used classes are marked cacheable and inherit from LinqObjectModel<T>):

        Test Case Failures:
        1) Docas.UnitTests.Common.ModelTest.T0300_HRM_JobRoleRelation : System.NullReferenceException : Der Objektverweis wurde nicht auf eine Objektinstanz festgelegt.
        bei Docas.UnitTests.Common.ModelTest.T0300_HRM_JobRoleRelation() in d:\dev\projects\cs\products\Docas\Docas.UnitTests\ModelTests.cs:Zeile 795.

        *This exception is only thrown on the last line of the testing method when caching is enabled.*
        */

        [Test]
        public void T0300_HRM_JobRoleRelation()
        {
            // get system user
            var u = User.FindOne(x => x.Name == "SYSTEM");
            Assert.IsNotNull(u);

            // get organisational unit
            var ou = OrganisationalUnit.FindOne(x => x.Id == 1);
            Assert.IsNotNull(ou);

            // get employee
            var emp = Employee.FindOne(x => x.Id == 1);
            Assert.IsNotNull(emp);

            // create employee job role relation
            EmployeeRoleRelation rel1 = EmployeeRoleRelation.New();
            rel1.CreatedBy = u.Id;
            rel1.Active = true;
            rel1.Start = DateTime.Now;
            rel1.Employee = emp;
            rel1.Save();

            // create job role relation
            JobRoleRelation rel = JobRoleRelation.New();
            rel.CreatedBy = u.Id;
            rel.Active = true;
            rel.Type = JobRoleRelationType.Manager;
            rel.OrganisationalUnit = ou;
            rel.Save();

            JobRole jr = JobRole.New();
            jr.CreatedBy = u.Id;
            jr.Code = "CEO";
            jr.Name = "CEO";
            jr.Description = "CEO";
            jr.JobRoleRelations.Add(rel);
            jr.EmployeeRoleRelations.Add(rel1);
            jr.Save();

            // get organisational unit
            ou = OrganisationalUnit.FindOne(x => x.Id == 1);
            Assert.IsNotNull(ou);

            Assert.AreEqual(1, ou.JobRoleRelations.Count);
            Assert.AreEqual("CEO", ou.JobRoleRelations[0].JobRole.Code);
            Assert.AreEqual(1, ou.JobRoleRelations[0].JobRole.EmployeeRoleRelations.Count);
            Assert.AreEqual("Mustermann", ou.JobRoleRelations[0].JobRole.EmployeeRoleRelations[0].Employee.Person.LastName); // error
        }

        /*
        [SQL-Recorder output]
        .T0300_HRM_JobRoleRelation --> BEGIN
        Select [Id],[DC],[UC],[DM],[UM],[AF],[LF],[USER_NAME],[USER_PASS],[PERSON_ID],[PROFILE_ID] From [DCS_USERS] Where [USER_NAME] = @USER_NAME_0;
        <Text><60>(@USER_NAME_0=SYSTEM:String)

        Select [Id],[DC],[UC],[DM],[UM],[UNIT_NAME],[UNIT_CODE],[COST_CENTRE],[CN],[UNIT_PARENT] From [REF_ORG_UNIT] Where [Id] = @Id_0;
        <Text><60>(@Id_0=1:Int64)

        Select [Id],[DC],[UC],[DM],[UM] From [HRM_EMPLOYEES] Where [Id] = @Id_0;
        <Text><60>(@Id_0=1:Int64)

        CREATE TABLE [REL_EMP_JOB_ROLE] (
        [Id] bigint IDENTITY NOT FOR REPLICATION NOT NULL PRIMARY KEY,
        [DC] datetime NOT NULL ,
        [UC] bigint NOT NULL ,
        [DM] datetime NULL ,
        [UM] bigint NULL ,
        [AF] bit NOT NULL ,
        [START_DATE] datetime NULL ,
        [END_DATE] datetime NULL ,
        [EMPLOYEE_ID] bigint NOT NULL ,
        [JOB_ROLE_ID] bigint NULL 
        );
        CREATE INDEX [IX_REL_EMP_JOB_ROLE_FK1] ON [REL_EMP_JOB_ROLE] ([EMPLOYEE_ID] ASC);
        CREATE INDEX [IX_REL_EMP_JOB_ROLE_FK2] ON [REL_EMP_JOB_ROLE] ([JOB_ROLE_ID] ASC);
        <Text><30>()

        Insert Into [REL_EMP_JOB_ROLE] ([DC],[UC],[UM],[AF],[START_DATE],[END_DATE],[EMPLOYEE_ID],[JOB_ROLE_ID]) Values (getdate(),@UC_0,@UM_1,@AF_2,@START_DATE_3,@END_DATE_4,@EMPLOYEE_ID_5,@JOB_ROLE_ID_6);
        select SCOPE_IDENTITY();
        <Text><30>(@UC_0=1:Int64,@UM_1=0:Int64,@AF_2=True:Boolean,@START_DATE_3=26.03.2008 19:53:19:DateTime,@END_DATE_4=<NULL>:DateTime,@EMPLOYEE_ID_5=1:Int64,@JOB_ROLE_ID_6=0:Int64)

        CREATE TABLE [REL_JOB_ROLE_ORG_UNIT] (
        [Id] bigint IDENTITY NOT FOR REPLICATION NOT NULL PRIMARY KEY,
        [DC] datetime NOT NULL ,
        [UC] bigint NOT NULL ,
        [DM] datetime NULL ,
        [UM] bigint NULL ,
        [AF] bit NOT NULL ,
        [RELATION_TYPE] int NOT NULL ,
        [ORG_UNIT_ID] bigint NOT NULL ,
        [JOB_ROLE_ID] bigint NOT NULL 
        );
        CREATE INDEX [IX_REL_JOB_ROLE_ORG_UNIT_FK1] ON [REL_JOB_ROLE_ORG_UNIT] ([ORG_UNIT_ID] ASC);
        CREATE INDEX [IX_REL_JOB_ROLE_ORG_UNIT_FK2] ON [REL_JOB_ROLE_ORG_UNIT] ([JOB_ROLE_ID] ASC);
        <Text><30>()

        Insert Into [REL_JOB_ROLE_ORG_UNIT] ([DC],[UC],[UM],[AF],[RELATION_TYPE],[ORG_UNIT_ID],[JOB_ROLE_ID]) Values (getdate(),@UC_0,@UM_1,@AF_2,@RELATION_TYPE_3,@ORG_UNIT_ID_4,@JOB_ROLE_ID_5);
        select SCOPE_IDENTITY();
        <Text><30>(@UC_0=1:Int64,@UM_1=0:Int64,@AF_2=True:Boolean,@RELATION_TYPE_3=Manager:Int32,@ORG_UNIT_ID_4=1:Int64,@JOB_ROLE_ID_5=0:Int64)

        CREATE TABLE [HRM_JOB_ROLES] (
        [Id] bigint IDENTITY NOT FOR REPLICATION NOT NULL PRIMARY KEY,
        [DC] datetime NOT NULL ,
        [UC] bigint NOT NULL ,
        [DM] datetime NULL ,
        [UM] bigint NULL ,
        [ROLE_NAME] nvarchar (64) NOT NULL ,
        [CODE] nvarchar (36) NOT NULL ,
        [DESCRIPTION] nvarchar (256) NOT NULL ,
        [ROLE_PARENT] bigint NULL 
        );
        CREATE INDEX [IX_HRM_JOB_ROLES_1] ON [HRM_JOB_ROLES] ([ROLE_NAME] ASC);
        CREATE UNIQUE INDEX [IX_HRM_JOB_ROLES_UK1] ON [HRM_JOB_ROLES] ([CODE] ASC);
        CREATE INDEX [IX_HRM_JOB_ROLES_FK1] ON [HRM_JOB_ROLES] ([ROLE_PARENT] ASC);
        <Text><30>()

        Insert Into [HRM_JOB_ROLES] ([DC],[UC],[UM],[ROLE_NAME],[CODE],[DESCRIPTION],[ROLE_PARENT]) Values (getdate(),@UC_0,@UM_1,@ROLE_NAME_2,@CODE_3,@DESCRIPTION_4,@ROLE_PARENT_5);
        select SCOPE_IDENTITY();
        <Text><30>(@UC_0=1:Int64,@UM_1=0:Int64,@ROLE_NAME_2=CEO:String,@CODE_3=CEO:String,@DESCRIPTION_4=CEO:String,@ROLE_PARENT_5=0:Int64)

        Update [REL_JOB_ROLE_ORG_UNIT] Set [UC]=@UC_0,[DM]=getdate(),[AF]=@AF_1,[RELATION_TYPE]=@RELATION_TYPE_2,[ORG_UNIT_ID]=@ORG_UNIT_ID_3,[JOB_ROLE_ID]=@JOB_ROLE_ID_4 Where [Id] = @Id_5;
        <Text><30>(@UC_0=1:Int64,@AF_1=True:Boolean,@RELATION_TYPE_2=Manager:Int32,@ORG_UNIT_ID_3=1:Int64,@JOB_ROLE_ID_4=1:Int64,@Id_5=1:Int64)

        Update [REL_EMP_JOB_ROLE] Set [UC]=@UC_0,[DM]=getdate(),[AF]=@AF_1,[START_DATE]=@START_DATE_2,[EMPLOYEE_ID]=@EMPLOYEE_ID_3,[JOB_ROLE_ID]=@JOB_ROLE_ID_4 Where [Id] = @Id_5;
        <Text><30>(@UC_0=1:Int64,@AF_1=True:Boolean,@START_DATE_2=26.03.2008 19:53:19:DateTime,@EMPLOYEE_ID_3=1:Int64,@JOB_ROLE_ID_4=1:Int64,@Id_5=1:Int64)

        Select [Id],[DC],[UC],[DM],[UM],[UNIT_NAME],[UNIT_CODE],[COST_CENTRE],[CN],[UNIT_PARENT] From [REF_ORG_UNIT] Where [Id] = @Id_0;
        <Text><60>(@Id_0=1:Int64)

        Select [Id],[DC],[UC],[DM],[UM],[AF],[RELATION_TYPE],[ORG_UNIT_ID],[JOB_ROLE_ID] From [REL_JOB_ROLE_ORG_UNIT] Where [ORG_UNIT_ID] = @ORG_UNIT_ID_0;
        <Text><60>(@ORG_UNIT_ID_0=1:Int64)

        Select [Id],[DC],[UC],[DM],[UM],[ROLE_NAME],[CODE],[DESCRIPTION],[ROLE_PARENT] From [HRM_JOB_ROLES] Where [Id] = @Id_0;
        <Text><60>(@Id_0=1:Int64)

        Select [Id],[DC],[UC],[DM],[UM],[AF],[START_DATE],[END_DATE],[EMPLOYEE_ID],[JOB_ROLE_ID] From [REL_EMP_JOB_ROLE] Where [JOB_ROLE_ID] = @JOB_ROLE_ID_0;
        <Text><60>(@JOB_ROLE_ID_0=1:Int64)

        *When caching is disabled the following additional SQL-statements are issued:*

        Select [Id],[DC],[UC],[DM],[UM] From [HRM_EMPLOYEES] Where [Id] = @Id_0;
        <Text><60>(@Id_0=1:Int64)

        select [Id],[DC],[UC],[DM],[UM],[NAME_FIRST],[NAME_LAST],[NAME_MIDDLE],[NAME_PREFIX],[NAME_POSTFIX],[GENDER],[DOB],[DOD],[EMPLOYEE_ID] from (select [Id],[DC],[UC],[DM],[UM],[NAME_FIRST],[NAME_LAST],[NAME_MIDDLE],[NAME_PREFIX],[NAME_POSTFIX],[GENDER],[DOB],[DOD],[EMPLOYEE_ID], ROW_NUMBER() OVER ( Order By [Id] ASC) as __rownumber__ From [DCS_PERSONS] Where [EMPLOYEE_ID] = @EMPLOYEE_ID_0) as T Where T.__rownumber__ >= 1 and T.__rownumber__ <= 1;
        <Text><60>(@EMPLOYEE_ID_0=1:Int64)
        */
    }
}
