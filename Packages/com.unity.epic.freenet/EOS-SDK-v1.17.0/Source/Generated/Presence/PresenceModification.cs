// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Presence
{
    public sealed partial class PresenceModification : Handle
    {
        public PresenceModification()
        {
        }

        public PresenceModification(System.IntPtr innerHandle) : base(innerHandle)
        {
        }

        public const int PresencemodificationDatarecordidApiLatest = 1;

        /// <summary>
        /// Most recent version of the <see cref="DeleteData" /> API.
        /// </summary>
        public const int PresencemodificationDeletedataApiLatest = 1;

        public const int PresencemodificationJoininfoMaxLength = PresenceInterface.DataMaxValueLength;

        /// <summary>
        /// The most recent version of the <see cref="SetData" /> API.
        /// </summary>
        public const int PresencemodificationSetdataApiLatest = 1;

        public const int PresencemodificationSetjoininfoApiLatest = 1;

        /// <summary>
        /// The most recent version of the <see cref="SetRawRichText" /> function.
        /// </summary>
        public const int PresencemodificationSetrawrichtextApiLatest = 1;

        /// <summary>
        /// The most recent version of the <see cref="SetStatus" /> API.
        /// </summary>
        public const int PresencemodificationSetstatusApiLatest = 1;

        /// <summary>
        /// Removes one or more rows of user-defined presence data for a local user. At least one DeleteDataInfo object
        /// must be specified.
        /// <seealso cref="PresenceInterface.DataMaxKeys" />
        /// <seealso cref="PresenceInterface.DataMaxKeyLength" />
        /// <seealso cref="PresenceInterface.DataMaxValueLength" />
        /// </summary>
        /// <param name="options">Object containing an array of new presence data.</param>
        /// <returns>
        /// Success if modification was added successfully, otherwise an error code related to the problem
        /// </returns>
        public Result DeleteData(ref PresenceModificationDeleteDataOptions options)
        {
            PresenceModificationDeleteDataOptionsInternal optionsInternal = new PresenceModificationDeleteDataOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_PresenceModification_DeleteData(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Release the memory associated with an <see cref="PresenceModification" /> handle. This must be called on Handles retrieved from <see cref="PresenceInterface.CreatePresenceModification" />.
        /// This can be safely called on a <see langword="null" /> presence modification handle. This also may be safely called while a call to SetPresence is still pending.
        /// <seealso cref="PresenceInterface.CreatePresenceModification" />
        /// </summary>
        /// <param name="presenceModificationHandle">The presence modification handle to release</param>
        public void Release()
        {
            Bindings.EOS_PresenceModification_Release(InnerHandle);
        }

        /// <summary>
        /// Modifies one or more rows of user-defined presence data for a local user. At least one InfoData object
        /// must be specified.
        /// <seealso cref="PresenceInterface.DataMaxKeys" />
        /// <seealso cref="PresenceInterface.DataMaxKeyLength" />
        /// <seealso cref="PresenceInterface.DataMaxValueLength" />
        /// </summary>
        /// <param name="options">Object containing an array of new presence data.</param>
        /// <returns>
        /// Success if modification was added successfully, otherwise an error code related to the problem
        /// </returns>
        public Result SetData(ref PresenceModificationSetDataOptions options)
        {
            PresenceModificationSetDataOptionsInternal optionsInternal = new PresenceModificationSetDataOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_PresenceModification_SetData(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Sets your new join info custom game-data string. This is a helper function for reading the presence data related to how a user can be joined.
        /// Its meaning is entirely application dependent.
        /// <seealso cref="PresencemodificationJoininfoMaxLength" />
        /// </summary>
        /// <param name="options">Object containing a join info string and associated user data</param>
        /// <returns>
        /// Success if modification was added successfully, otherwise an error code related to the problem
        /// </returns>
        public Result SetJoinInfo(ref PresenceModificationSetJoinInfoOptions options)
        {
            PresenceModificationSetJoinInfoOptionsInternal optionsInternal = new PresenceModificationSetJoinInfoOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_PresenceModification_SetJoinInfo(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Modifies a user's Rich Presence string to a new state. This is the exact value other users will see
        /// when they query the local user's presence.
        /// <seealso cref="PresenceInterface.RichTextMaxValueLength" />
        /// </summary>
        /// <param name="options">Object containing properties related to setting a user's RichText string</param>
        /// <returns>
        /// Success if modification was added successfully, otherwise an error code related to the problem
        /// </returns>
        public Result SetRawRichText(ref PresenceModificationSetRawRichTextOptions options)
        {
            PresenceModificationSetRawRichTextOptionsInternal optionsInternal = new PresenceModificationSetRawRichTextOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_PresenceModification_SetRawRichText(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }

        /// <summary>
        /// Modifies a user's online status to be the new state.
        /// </summary>
        /// <param name="options">Object containing properties related to setting a user's Status</param>
        /// <returns>
        /// Success if modification was added successfully, otherwise an error code related to the problem
        /// </returns>
        public Result SetStatus(ref PresenceModificationSetStatusOptions options)
        {
            PresenceModificationSetStatusOptionsInternal optionsInternal = new PresenceModificationSetStatusOptionsInternal();
            optionsInternal.Set(ref options);

            var funcResult = Bindings.EOS_PresenceModification_SetStatus(InnerHandle, ref optionsInternal);

            Helper.Dispose(ref optionsInternal);

            return funcResult;
        }
    }
}