using System;
using System.Diagnostics;

namespace UnitTests
{
    [DebuggerStepThrough()]
    internal static class Assert
    {
        internal static void AreEqual<T>(T expected, T actual, string parameterName) where T : struct
        {
            if (expected.Equals(actual) == false)
            {
                string message;

                message = BuildErrorMessage(expected, actual, parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        internal static void AreEqual(string expected, string actual, string parameterName)
        {
            if (expected != actual)
            {
                string message;
                message = BuildErrorMessage(expected, actual, parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        private static string BuildErrorMessage<T>(T expected, T actual, string parameterName)
        {
            string message;
            StackTrace stackTrace;
            StackFrame stackFrame;

            stackTrace = new StackTrace();
            stackFrame = stackTrace.GetFrame(2);
            message = string.Format("{3}()\n{2} was wrong!\nExpected: <{0}>\nbut was: <{1}>", expected, actual, parameterName, stackFrame.GetMethod().Name);
            return message;
        }

        //internal static void AreEqual(string expected, string actual)
        //{
        //    AreEqual(expected, actual, string.Empty);
        //}

        internal static void IsNotNull(object obj, string parameterName)
        {
            if (obj == null)
            {
                string message;

                message = BuildErrorMessage("not null", "null", parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        internal static void IsNull(object obj, string parameterName)
        {
            if (obj != null)
            {
                string message;

                message = BuildErrorMessage("null", "not null", parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        internal static void IsTrue(bool actual, string parameterName)
        {
            if (actual == false)
            {
                string message;

                message = BuildErrorMessage(true, actual, parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }
    }

    internal class AssertionException : Exception
    {
        public AssertionException()
            : base()
        {
        }

        public AssertionException(string message)
            : base(message)
        {
        }
    }
}
