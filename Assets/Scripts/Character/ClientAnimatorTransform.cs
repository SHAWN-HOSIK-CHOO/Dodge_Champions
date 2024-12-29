using Unity.Netcode;
using Unity.Netcode.Components;

namespace Character
{
   public class ClientAnimatorTransform : NetworkAnimator
   {
      protected override bool OnIsServerAuthoritative()
      {
         return false;
      }
   }
}
