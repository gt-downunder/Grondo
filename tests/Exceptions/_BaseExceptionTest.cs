namespace Grondo.Tests.Exceptions
{
    [TestClass]
    [TestCategory("Exceptions")]
    public class BaseExceptionTest : BaseTest
    {
        protected static void ThrowException(Exception ex) => throw ex;
    }
}
