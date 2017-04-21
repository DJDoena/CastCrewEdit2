using System;
using System.Diagnostics;

namespace UnitTests
{
    [DebuggerStepThrough()]
    internal static class Assert
    {
        internal static void AreEqual<T>(T expected, T actual, String parameterName) where T : struct
        {
            if (expected.Equals(actual) == false)
            {
                String message;

                message = BuildErrorMessage(expected, actual, parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        internal static void AreEqual(String expected, String actual, String parameterName)
        {
            if (expected != actual)
            {
                String message;
                message = BuildErrorMessage(expected, actual, parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        private static String BuildErrorMessage<T>(T expected, T actual, String parameterName)
        {
            String message;
            StackTrace stackTrace;
            StackFrame stackFrame;

            stackTrace = new StackTrace();
            stackFrame = stackTrace.GetFrame(2);
            message = String.Format("{3}()\n{2} was wrong!\nExpected: <{0}>\nbut was: <{1}>", expected, actual, parameterName, stackFrame.GetMethod().Name);
            return message;
        }

        //internal static void AreEqual(String expected, String actual)
        //{
        //    AreEqual(expected, actual, String.Empty);
        //}

        internal static void IsNotNull(Object obj, String parameterName)
        {
            if (obj == null)
            {
                String message;

                message = BuildErrorMessage("not null", "null", parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        internal static void IsNull(Object obj, String parameterName)
        {
            if (obj != null)
            {
                String message;

                message = BuildErrorMessage("null", "not null", parameterName);
                Debug.Assert(false, message);
                throw (new AssertionException(message));
            }
        }

        internal static void IsTrue(Boolean actual, String parameterName)
        {
            if (actual == false)
            {
                String message;

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

        public AssertionException(String message)
            : base(message)
        {
        }
    }
}
