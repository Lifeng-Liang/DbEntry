Imports Lephone.Data
Imports Lephone.UnitTest.Linq
Imports NUnit.Framework

<TestFixture()>
Public Class CommonTest
    Inherits DataTestBase

    <Test()>
    Public Sub Test1()
        Dim p = RealOperateTest.Person.FindOne(Function(o) o.Id = 2)
        Assert.AreEqual(2, p.Id)
        Assert.AreEqual("Jerry", p.FirstName)
    End Sub

    <Test()>
    Public Sub Test2()
        Dim list = RealOperateTest.Person.Find(Function(p) p.Id > 1, Function(p) p.Id)
        Assert.AreEqual(2, list.Count)
        Assert.AreEqual("Jerry", list(0).FirstName)
        Assert.AreEqual("Mike", list(1).FirstName)
    End Sub

    <Test()>
    Public Sub Test3()

        Dim p1 = RealOperateTest.Person.FindOne(Function(o) o.Id = 2)
        Dim p2 = RealOperateTest.Person.FindOne(Function(o) o.Id = 3)
        p1.FirstName = "haha"
        p2.FirstName = "uu"

        DbEntry.UsingTransaction(Sub()
                                     p1.Save()
                                     p2.Save()
                                 End Sub)

        Dim p3 = RealOperateTest.Person.FindOne(Function(o) o.Id = 2)
        Dim p4 = RealOperateTest.Person.FindOne(Function(o) o.Id = 3)

        Assert.AreEqual("haha", p3.FirstName)
        Assert.AreEqual("uu", p4.FirstName)
    End Sub

    <Test()>
    Public Sub Test4()
        Dim p1 = RealOperateTest.Person.FindOne(Function(o) o.Id = 2)
        Dim p2 = RealOperateTest.Person.FindOne(Function(o) o.Id = 3)
        p1.FirstName = "haha"
        p2.FirstName = "uu"

        Try
            DbEntry.UsingTransaction(
                Sub()
                    p1.Save()
                    p2.Save()
                    Throw New ApplicationException()
                End Sub)
        Catch ex As Exception
        End Try

        Dim p3 = RealOperateTest.Person.FindOne(Function(o) o.Id = 2)
        Dim p4 = RealOperateTest.Person.FindOne(Function(o) o.Id = 3)

        Assert.AreEqual("Jerry", p3.FirstName)
        Assert.AreEqual("Mike", p4.FirstName)
    End Sub

End Class
