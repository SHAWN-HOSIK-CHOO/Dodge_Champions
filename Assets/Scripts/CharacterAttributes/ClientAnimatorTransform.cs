using Unity.Netcode;
using Unity.Netcode.Components;

namespace CharacterAttributes
{
   public class ClientAnimatorTransform : NetworkAnimator
   {
      protected override bool OnIsServerAuthoritative()
      {
         return false;
      }
   }
}
