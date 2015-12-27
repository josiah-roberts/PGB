﻿using System;

namespace PGBLib.IO
{
    public class OperationProgressDetails : EventArgs
    {   
        public OperationProgressType ProgressType { get; }
        public IOOperation Operation { get; }
        public bool Completed { get; }
        public long BytesTransferred { get; }
        public long BytesTotal { get; }
        public Exception Error { get; }
        public double PercentComplete
        {
            get
            {
                if (BytesTotal == 0)
                    return 0;
                return ((double)BytesTransferred) / ((double)BytesTotal) * 100;
            }
        }

        public OperationProgressDetails(IOOperation operation, bool completed)
        {
            Operation = operation;
            Completed = completed;

            ProgressType = completed ? OperationProgressType.Completed : OperationProgressType.Generic;
        }

        public OperationProgressDetails(IOOperation operation, Exception error)
        {
            Operation = operation;
            this.Error = error;

            ProgressType = error != null ? OperationProgressType.Errored : OperationProgressType.Generic;
        }

        public OperationProgressDetails(IOOperation operation, long bytesTransfered, long bytesTotal)
        {
            Operation = operation;
            BytesTransferred = bytesTransfered;
            BytesTotal = bytesTotal;

            ProgressType = OperationProgressType.InProgress;
        }

        public override string ToString()
        {
            return Operation.ToString() + " is " + ProgressType.ToString();
        }
    }

    public enum OperationProgressType
    {
        Generic,
        Completed,
        Errored,
        InProgress
    }
}
