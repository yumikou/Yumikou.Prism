using System;
using System.Collections.Generic;
using System.Text;

namespace Prism.Regions
{
    public class RequestCreateResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestCreateResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public RequestCreateResult(RequestCreateContext context, bool result)
        {
            this.Context = context;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestCreateResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public RequestCreateResult(RequestCreateContext context, Exception error)
        {
            this.Context = context;
            this.Error = error;
            this.Result = false;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public bool Result { get; private set; }

        /// <summary>
        /// Gets an exception that occurred while request create a region view.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the request create context.
        /// </summary>
        /// <value>The request create context.</value>
        public RequestCreateContext Context { get; private set; }
    }
}
