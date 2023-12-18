using System;

namespace PartialDeathdrop
{
    public class PdUnhookException : Exception
    {
        public PdUnhookException(string message) : base(message)
        {
        }
    }
}