// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.ProgressionSnapshot
{
    public sealed partial class ProgressionSnapshotInterface : Handle
    {
        public ProgressionSnapshotInterface()
        {
        }

        public ProgressionSnapshotInterface(System.IntPtr innerHandle) : base(innerHandle)
        {
        }

        public const int AddprogressionApiLatest = 1;

        public const int BeginsnapshotApiLatest = 1;

        public const int DeletesnapshotApiLatest = 1;

        public const int EndsnapshotApiLatest = 1;

        public const int InvalidProgressionsnapshotid = 0;

        public const int SubmitsnapshotApiLatest = 1;

        /// <summary>
        /// Stores a Key/Value pair in memory for a given snapshot.
        /// If multiple calls happen with the same key, the last invocation wins, overwriting the previous value for that
        /// given key.
        /// 
        /// The order in which the Key/Value pairs are added is stored as is for later retrieval/display.
        /// Ideally, you would make multiple calls to AddProgression() followed by a single call to SubmitSnapshot().
        /// </summary>
        /// <returns>
        /// <see cref="Result.Success" /> when successful; otherwise, <see cref="Result.NotFound" />
        /// </returns>
        public Result AddProgression(ref AddProgressionOptions options)
        {
            AddProgressionOptionsInternal optionsInternal = new AddProgressionOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_ProgressionSnapshot_AddProgression(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Creates a new progression-snapshot resource for a given user.
        /// </summary>
        /// <param name="options">Object containing properties that identifies the PUID this Snapshot will belong to.</param>
        /// <param name="outSnapshotId">A progression-snapshot identifier output parameter. Use that identifier to reference the snapshot in the other APIs.</param>
        /// <returns>
        /// <see cref="Result.Success" /> when successful.
        /// <see cref="Result.ProgressionSnapshotSnapshotIdUnavailable" /> when no IDs are available. This is irrecoverable state.
        /// </returns>
        public Result BeginSnapshot(ref BeginSnapshotOptions options, out uint outSnapshotId)
        {
            BeginSnapshotOptionsInternal optionsInternal = new BeginSnapshotOptionsInternal();
            optionsInternal.Set(ref options);

            outSnapshotId = Helper.GetDefault<uint>();

            var funcResult = Bindings.EOS_ProgressionSnapshot_BeginSnapshot(InnerHandle, ref optionsInternal, ref outSnapshotId);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Wipes out all progression data for the given user from the service. However, any previous progression data that haven't
        /// been submitted yet are retained.
        /// </summary>
        public void DeleteSnapshot(ref DeleteSnapshotOptions options, object clientData, OnDeleteSnapshotCallback completionDelegate)
        {
            DeleteSnapshotOptionsInternal optionsInternal = new DeleteSnapshotOptionsInternal();
            optionsInternal.Set(ref options);

            var clientDataAddress = System.IntPtr.Zero;

            var completionDelegateInternal = new OnDeleteSnapshotCallbackInternal(OnDeleteSnapshotCallbackInternalImplementation);
            Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

            Bindings.EOS_ProgressionSnapshot_DeleteSnapshot(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

            Helper.Dispose(ref optionsInternal);
        }

        /// <summary>
        /// Cleans up and releases resources associated with the given progression snapshot identifier.
        /// </summary>
        /// <returns>
        /// <see cref="Result.Success" /> when successful; otherwise, <see cref="Result.NotFound" />
        /// </returns>
        public Result EndSnapshot(ref EndSnapshotOptions options)
        {
            EndSnapshotOptionsInternal optionsInternal = new EndSnapshotOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_ProgressionSnapshot_EndSnapshot(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Saves the previously added Key/Value pairs of a given Snapshot to the service.
        /// 
        /// Note: This will overwrite any prior progression data stored with the service that's associated with the user.
        /// </summary>
        public void SubmitSnapshot(ref SubmitSnapshotOptions options, object clientData, OnSubmitSnapshotCallback completionDelegate)
        {
            SubmitSnapshotOptionsInternal optionsInternal = new SubmitSnapshotOptionsInternal();
            optionsInternal.Set(ref options);

            var clientDataAddress = System.IntPtr.Zero;

            var completionDelegateInternal = new OnSubmitSnapshotCallbackInternal(OnSubmitSnapshotCallbackInternalImplementation);
            Helper.AddCallback(out clientDataAddress, clientData, completionDelegate, completionDelegateInternal);

            Bindings.EOS_ProgressionSnapshot_SubmitSnapshot(InnerHandle, ref optionsInternal, clientDataAddress, completionDelegateInternal);

            Helper.Dispose(ref optionsInternal);
        }

        [MonoPInvokeCallback(typeof(OnDeleteSnapshotCallbackInternal))]
        internal static void OnDeleteSnapshotCallbackInternalImplementation(ref DeleteSnapshotCallbackInfoInternal data)
        {
            OnDeleteSnapshotCallback callback;
            DeleteSnapshotCallbackInfo callbackInfo;
            if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
            {
                callback(ref callbackInfo);
            }
        }

        [MonoPInvokeCallback(typeof(OnSubmitSnapshotCallbackInternal))]
        internal static void OnSubmitSnapshotCallbackInternalImplementation(ref SubmitSnapshotCallbackInfoInternal data)
        {
            OnSubmitSnapshotCallback callback;
            SubmitSnapshotCallbackInfo callbackInfo;
            if (Helper.TryGetAndRemoveCallback(ref data, out callback, out callbackInfo))
            {
                callback(ref callbackInfo);
            }
        }
    }
}