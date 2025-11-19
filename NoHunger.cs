using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ModLoader;

namespace NoHunger;

public class NoHunger : Mod
{
    public override void Load()
    {
        IL_Player.UpdateHungerBuffs += Hook_UpdateHungerBuffs;
        IL_Player.UpdateStarvingState += Hook_UpdateStarvingState;
        IL_Player.UpdateBuffs += (ILContext) => { }; // for some incomprehensible reason this is necessary for the UpdateStarvingState hook to do anything
    }

    private static void Hook_UpdateHungerBuffs(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            while (c.TryGotoNext(MoveType.After, i => i.MatchLdsfld<Main>(nameof(Main.dontStarveWorld))))
            {
                c.EmitLdcI4(0);
                c.Index -= 2;
                c.Remove();
            }
        }
        catch (Exception e)
        {
            throw new ILPatchFailureException(ModContent.GetInstance<NoHunger>(), il, e);
        }
    }

    private void Hook_UpdateStarvingState(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After, i => i.Previous?.MatchLdloc0() ?? false && i.MatchBrtrue(out _));
            c.Prev.MatchBrtrue(out ILLabel label);
            c.EmitBr(label);
        }
        catch (Exception e)
        {
            throw new ILPatchFailureException(ModContent.GetInstance<NoHunger>(), il, e);
        }
    }
}