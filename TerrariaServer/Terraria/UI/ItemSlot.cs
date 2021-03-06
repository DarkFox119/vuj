﻿// Decompiled with JetBrains decompiler
// Type: Terraria.UI.ItemSlot
// Assembly: TerrariaServer, Version=1.3.5.3, Culture=neutral, PublicKeyToken=null
// MVID: 8A63A7A2-328D-424C-BC9D-BF23F93646F7
// Assembly location: H:\Steam\steamapps\common\Terraria\TerrariaServer.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;

namespace Terraria.UI
{
  public class ItemSlot
  {
    public static bool ShiftForcedOn = false;
    private static Item[] singleSlotArray = new Item[1];
    private static bool[] canFavoriteAt = new bool[23];
    private static float[] inventoryGlowHue = new float[58];
    private static int[] inventoryGlowTime = new int[58];
    private static float[] inventoryGlowHueChest = new float[58];
    private static int[] inventoryGlowTimeChest = new int[58];
    private static int _customCurrencyForSavings = -1;
    private static int dyeSlotCount = 0;
    private static int accSlotCount = 0;
    public static float CircularRadialOpacity = 0.0f;
    public static float QuicksRadialOpacity = 0.0f;

    public static bool ShiftInUse
    {
      get
      {
        if (!Main.keyState.PressingShift())
          return ItemSlot.ShiftForcedOn;
        return true;
      }
    }

    static ItemSlot()
    {
      ItemSlot.canFavoriteAt[0] = true;
      ItemSlot.canFavoriteAt[1] = true;
      ItemSlot.canFavoriteAt[2] = true;
    }

    public static void SetGlow(int index, float hue, bool chest)
    {
      if (chest)
      {
        ItemSlot.inventoryGlowTimeChest[index] = 300;
        ItemSlot.inventoryGlowHueChest[index] = hue;
      }
      else
      {
        ItemSlot.inventoryGlowTime[index] = 300;
        ItemSlot.inventoryGlowHue[index] = hue;
      }
    }

    public static void UpdateInterface()
    {
      if (!Main.playerInventory || Main.player[Main.myPlayer].talkNPC == -1)
        ItemSlot._customCurrencyForSavings = -1;
      for (int index = 0; index < ItemSlot.inventoryGlowTime.Length; ++index)
      {
        if (ItemSlot.inventoryGlowTime[index] > 0)
        {
          --ItemSlot.inventoryGlowTime[index];
          if (ItemSlot.inventoryGlowTime[index] == 0)
            ItemSlot.inventoryGlowHue[index] = 0.0f;
        }
      }
      for (int index = 0; index < ItemSlot.inventoryGlowTimeChest.Length; ++index)
      {
        if (ItemSlot.inventoryGlowTimeChest[index] > 0)
        {
          --ItemSlot.inventoryGlowTimeChest[index];
          if (ItemSlot.inventoryGlowTimeChest[index] == 0)
            ItemSlot.inventoryGlowHueChest[index] = 0.0f;
        }
      }
    }

    public static void Handle(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      ItemSlot.Handle(ItemSlot.singleSlotArray, context, 0);
      inv = ItemSlot.singleSlotArray[0];
      Recipe.FindRecipes();
    }

    public static void Handle(Item[] inv, int context = 0, int slot = 0)
    {
      ItemSlot.OverrideHover(inv, context, slot);
      if (Main.mouseLeftRelease && Main.mouseLeft)
      {
        ItemSlot.LeftClick(inv, context, slot);
        Recipe.FindRecipes();
      }
      else
        ItemSlot.RightClick(inv, context, slot);
      ItemSlot.MouseHover(inv, context, slot);
    }

    public static void OverrideHover(Item[] inv, int context = 0, int slot = 0)
    {
      Item obj = inv[slot];
      if (ItemSlot.ShiftInUse && obj.type > 0 && (obj.stack > 0 && !inv[slot].favorited))
      {
        switch (context)
        {
          case 0:
          case 1:
          case 2:
            if (Main.npcShop > 0 && !obj.favorited)
            {
              Main.cursorOverride = 10;
              break;
            }
            if (Main.player[Main.myPlayer].chest != -1)
            {
              if (ChestUI.TryPlacingInChest(obj, true))
              {
                Main.cursorOverride = 9;
                break;
              }
              break;
            }
            Main.cursorOverride = 6;
            break;
          case 3:
          case 4:
            if (Main.player[Main.myPlayer].ItemSpace(obj))
            {
              Main.cursorOverride = 8;
              break;
            }
            break;
          case 5:
          case 8:
          case 9:
          case 10:
          case 11:
          case 12:
          case 16:
          case 17:
          case 18:
          case 19:
          case 20:
            if (Main.player[Main.myPlayer].ItemSpace(inv[slot]))
            {
              Main.cursorOverride = 7;
              break;
            }
            break;
        }
      }
      // ISSUE: explicit reference operation
      if (!((KeyboardState) @Main.keyState).IsKeyDown(Main.FavoriteKey) || !ItemSlot.canFavoriteAt[context])
        return;
      if (obj.type > 0 && obj.stack > 0 && Main.drawingPlayerChat)
      {
        Main.cursorOverride = 2;
      }
      else
      {
        if (obj.type <= 0 || obj.stack <= 0)
          return;
        Main.cursorOverride = 3;
      }
    }

    private static bool OverrideLeftClick(Item[] inv, int context = 0, int slot = 0)
    {
      Item I = inv[slot];
      if (Main.cursorOverride == 2)
      {
        if (ChatManager.AddChatText(Main.fontMouseText, ItemTagHandler.GenerateTag(I), Vector2.get_One()))
          Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
        return true;
      }
      if (Main.cursorOverride == 3)
      {
        if (!ItemSlot.canFavoriteAt[context])
          return false;
        I.favorited = !I.favorited;
        Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
        return true;
      }
      if (Main.cursorOverride == 7)
      {
        inv[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inv[slot], false, true);
        Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
        return true;
      }
      if (Main.cursorOverride == 8)
      {
        inv[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inv[slot], false, true);
        if (Main.player[Main.myPlayer].chest > -1)
          NetMessage.SendData(32, -1, -1, (NetworkText) null, Main.player[Main.myPlayer].chest, (float) slot, 0.0f, 0.0f, 0, 0, 0);
        return true;
      }
      if (Main.cursorOverride != 9)
        return false;
      ChestUI.TryPlacingInChest(inv[slot], false);
      return true;
    }

    public static void LeftClick(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      ItemSlot.LeftClick(ItemSlot.singleSlotArray, context, 0);
      inv = ItemSlot.singleSlotArray[0];
    }

    public static void LeftClick(Item[] inv, int context = 0, int slot = 0)
    {
      if (ItemSlot.OverrideLeftClick(inv, context, slot))
        return;
      inv[slot].newAndShiny = false;
      Player player = Main.player[Main.myPlayer];
      bool flag = false;
      if ((uint) context <= 4U)
        flag = player.chest == -1;
      if (ItemSlot.ShiftInUse & flag)
      {
        ItemSlot.SellOrTrash(inv, context, slot);
      }
      else
      {
        if (player.itemAnimation != 0 || player.itemTime != 0)
          return;
        switch (ItemSlot.PickItemMovementAction(inv, context, slot, Main.mouseItem))
        {
          case 0:
            if (context == 6 && Main.mouseItem.type != 0)
              inv[slot].SetDefaults(0, false);
            Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
            if (inv[slot].stack > 0)
            {
              if (context != 0)
              {
                if ((uint) (context - 8) <= 4U || (uint) (context - 16) <= 1U)
                  AchievementsHelper.HandleOnEquip(player, inv[slot], context);
              }
              else
                AchievementsHelper.NotifyItemPickup(player, inv[slot]);
            }
            if (inv[slot].type == 0 || inv[slot].stack < 1)
              inv[slot] = new Item();
            if (Main.mouseItem.IsTheSameAs(inv[slot]))
            {
              Utils.Swap<bool>(ref inv[slot].favorited, ref Main.mouseItem.favorited);
              if (inv[slot].stack != inv[slot].maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack)
              {
                if (Main.mouseItem.stack + inv[slot].stack <= Main.mouseItem.maxStack)
                {
                  inv[slot].stack += Main.mouseItem.stack;
                  Main.mouseItem.stack = 0;
                }
                else
                {
                  int num = Main.mouseItem.maxStack - inv[slot].stack;
                  inv[slot].stack += num;
                  Main.mouseItem.stack -= num;
                }
              }
            }
            if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
              Main.mouseItem = new Item();
            if (Main.mouseItem.type > 0 || inv[slot].type > 0)
            {
              Recipe.FindRecipes();
              Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            }
            if (context == 3 && Main.netMode == 1)
            {
              NetMessage.SendData(32, -1, -1, (NetworkText) null, player.chest, (float) slot, 0.0f, 0.0f, 0, 0, 0);
              break;
            }
            break;
          case 1:
            if (Main.mouseItem.stack == 1 && Main.mouseItem.type > 0 && (inv[slot].type > 0 && inv[slot].IsNotTheSameAs(Main.mouseItem)))
            {
              Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
              Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
              if (inv[slot].stack > 0)
              {
                if (context != 0)
                {
                  if ((uint) (context - 8) <= 4U || (uint) (context - 16) <= 1U)
                  {
                    AchievementsHelper.HandleOnEquip(player, inv[slot], context);
                    break;
                  }
                  break;
                }
                AchievementsHelper.NotifyItemPickup(player, inv[slot]);
                break;
              }
              break;
            }
            if (Main.mouseItem.type == 0 && inv[slot].type > 0)
            {
              Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
              if (inv[slot].type == 0 || inv[slot].stack < 1)
                inv[slot] = new Item();
              if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
                Main.mouseItem = new Item();
              if (Main.mouseItem.type > 0 || inv[slot].type > 0)
              {
                Recipe.FindRecipes();
                Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
                break;
              }
              break;
            }
            if (Main.mouseItem.type > 0 && inv[slot].type == 0)
            {
              if (Main.mouseItem.stack == 1)
              {
                Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
                if (inv[slot].type == 0 || inv[slot].stack < 1)
                  inv[slot] = new Item();
                if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
                  Main.mouseItem = new Item();
                if (Main.mouseItem.type > 0 || inv[slot].type > 0)
                {
                  Recipe.FindRecipes();
                  Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
                }
              }
              else
              {
                --Main.mouseItem.stack;
                inv[slot].SetDefaults(Main.mouseItem.type, false);
                Recipe.FindRecipes();
                Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
              }
              if (inv[slot].stack > 0)
              {
                if (context != 0)
                {
                  if ((uint) (context - 8) <= 4U || (uint) (context - 16) <= 1U)
                  {
                    AchievementsHelper.HandleOnEquip(player, inv[slot], context);
                    break;
                  }
                  break;
                }
                AchievementsHelper.NotifyItemPickup(player, inv[slot]);
                break;
              }
              break;
            }
            break;
          case 2:
            if (Main.mouseItem.stack == 1 && (int) Main.mouseItem.dye > 0 && (inv[slot].type > 0 && inv[slot].type != Main.mouseItem.type))
            {
              Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
              Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
              if (inv[slot].stack > 0)
              {
                if (context != 0)
                {
                  if ((uint) (context - 8) <= 4U || (uint) (context - 16) <= 1U)
                  {
                    AchievementsHelper.HandleOnEquip(player, inv[slot], context);
                    break;
                  }
                  break;
                }
                AchievementsHelper.NotifyItemPickup(player, inv[slot]);
                break;
              }
              break;
            }
            if (Main.mouseItem.type == 0 && inv[slot].type > 0)
            {
              Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
              if (inv[slot].type == 0 || inv[slot].stack < 1)
                inv[slot] = new Item();
              if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
                Main.mouseItem = new Item();
              if (Main.mouseItem.type > 0 || inv[slot].type > 0)
              {
                Recipe.FindRecipes();
                Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
                break;
              }
              break;
            }
            if ((int) Main.mouseItem.dye > 0 && inv[slot].type == 0)
            {
              if (Main.mouseItem.stack == 1)
              {
                Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
                if (inv[slot].type == 0 || inv[slot].stack < 1)
                  inv[slot] = new Item();
                if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
                  Main.mouseItem = new Item();
                if (Main.mouseItem.type > 0 || inv[slot].type > 0)
                {
                  Recipe.FindRecipes();
                  Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
                }
              }
              else
              {
                --Main.mouseItem.stack;
                inv[slot].SetDefaults(Main.mouseItem.type, false);
                Recipe.FindRecipes();
                Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
              }
              if (inv[slot].stack > 0)
              {
                if (context != 0)
                {
                  if ((uint) (context - 8) <= 4U || (uint) (context - 16) <= 1U)
                  {
                    AchievementsHelper.HandleOnEquip(player, inv[slot], context);
                    break;
                  }
                  break;
                }
                AchievementsHelper.NotifyItemPickup(player, inv[slot]);
                break;
              }
              break;
            }
            break;
          case 3:
            Main.mouseItem.netDefaults(inv[slot].netID);
            if (inv[slot].buyOnce)
              Main.mouseItem.Prefix((int) inv[slot].prefix);
            else
              Main.mouseItem.Prefix(-1);
            Main.mouseItem.position = Vector2.op_Subtraction(player.Center, Vector2.op_Division(new Vector2((float) Main.mouseItem.width, (float) Main.mouseItem.headSlot), 2f));
            ItemText.NewText(Main.mouseItem, Main.mouseItem.stack, false, false);
            if (inv[slot].buyOnce)
            {
              Item obj = inv[slot];
              int num1 = obj.stack - 1;
              int num2 = num1;
              obj.stack = num2;
              if (num1 <= 0)
                inv[slot].SetDefaults(0, false);
            }
            if (inv[slot].value > 0)
            {
              Main.PlaySound(18, -1, -1, 1, 1f, 0.0f);
              break;
            }
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            break;
          case 4:
            Chest chest = Main.instance.shop[Main.npcShop];
            if (player.SellItem(Main.mouseItem.value, Main.mouseItem.stack))
            {
              chest.AddShop(Main.mouseItem);
              Main.mouseItem.SetDefaults(0, false);
              Main.PlaySound(18, -1, -1, 1, 1f, 0.0f);
            }
            else if (Main.mouseItem.value == 0)
            {
              chest.AddShop(Main.mouseItem);
              Main.mouseItem.SetDefaults(0, false);
              Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            }
            Recipe.FindRecipes();
            break;
        }
        if ((uint) context <= 2U || context == 5)
          return;
        inv[slot].favorited = false;
      }
    }

    private static void SellOrTrash(Item[] inv, int context, int slot)
    {
      Player player = Main.player[Main.myPlayer];
      if (inv[slot].type <= 0)
        return;
      if (Main.npcShop > 0 && !inv[slot].favorited)
      {
        Chest chest = Main.instance.shop[Main.npcShop];
        if (inv[slot].type >= 71 && inv[slot].type <= 74)
          return;
        if (player.SellItem(inv[slot].value, inv[slot].stack))
        {
          chest.AddShop(inv[slot]);
          inv[slot].SetDefaults(0, false);
          Main.PlaySound(18, -1, -1, 1, 1f, 0.0f);
          Recipe.FindRecipes();
        }
        else
        {
          if (inv[slot].value != 0)
            return;
          chest.AddShop(inv[slot]);
          inv[slot].SetDefaults(0, false);
          Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
          Recipe.FindRecipes();
        }
      }
      else
      {
        if (inv[slot].favorited || ItemSlot.Options.DisableLeftShiftTrashCan)
          return;
        Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
        player.trashItem = inv[slot].Clone();
        inv[slot].SetDefaults(0, false);
        if (context == 3 && Main.netMode == 1)
          NetMessage.SendData(32, -1, -1, (NetworkText) null, player.chest, (float) slot, 0.0f, 0.0f, 0, 0, 0);
        Recipe.FindRecipes();
      }
    }

    private static string GetOverrideInstructions(Item[] inv, int context, int slot)
    {
      Player player = Main.player[Main.myPlayer];
      if (inv[slot].type > 0 && inv[slot].stack > 0)
      {
        if (!inv[slot].favorited)
        {
          switch (context)
          {
            case 0:
            case 1:
            case 2:
              if (Main.npcShop > 0 && !inv[slot].favorited)
                return Lang.misc[75].Value;
              if (Main.player[Main.myPlayer].chest == -1)
                return Lang.misc[74].Value;
              if (ChestUI.TryPlacingInChest(inv[slot], true))
                return Lang.misc[76].Value;
              break;
            case 3:
            case 4:
              if (Main.player[Main.myPlayer].ItemSpace(inv[slot]))
                return Lang.misc[76].Value;
              break;
            case 5:
            case 8:
            case 9:
            case 10:
            case 11:
            case 12:
            case 16:
            case 17:
            case 18:
            case 19:
            case 20:
              if (Main.player[Main.myPlayer].ItemSpace(inv[slot]))
                return Lang.misc[68].Value;
              break;
          }
        }
        bool flag = false;
        if ((uint) context <= 4U)
          flag = player.chest == -1;
        if (flag)
        {
          if (Main.npcShop > 0 && !inv[slot].favorited)
          {
            Chest chest = Main.instance.shop[Main.npcShop];
            if (inv[slot].type >= 71 && inv[slot].type <= 74)
              return "";
            return Lang.misc[75].Value;
          }
          if (!inv[slot].favorited && !ItemSlot.Options.DisableLeftShiftTrashCan)
            return Lang.misc[74].Value;
        }
      }
      return "";
    }

    public static int PickItemMovementAction(Item[] inv, int context, int slot, Item checkItem)
    {
      Player player = Main.player[Main.myPlayer];
      int num = -1;
      if (context == 0)
        num = 0;
      else if (context == 1)
      {
        if (checkItem.type == 0 || checkItem.type == 71 || (checkItem.type == 72 || checkItem.type == 73) || checkItem.type == 74)
          num = 0;
      }
      else if (context == 2)
      {
        if ((checkItem.type == 0 || checkItem.ammo > 0 || checkItem.bait > 0) && !checkItem.notAmmo || checkItem.type == 530)
          num = 0;
      }
      else if (context == 3)
        num = 0;
      else if (context == 4)
        num = 0;
      else if (context == 5)
      {
        if (checkItem.Prefix(-3) || checkItem.type == 0)
          num = 0;
      }
      else if (context == 6)
        num = 0;
      else if (context == 7)
      {
        if (checkItem.material || checkItem.type == 0)
          num = 0;
      }
      else if (context == 8)
      {
        if (checkItem.type == 0 || checkItem.headSlot > -1 && slot == 0 || (checkItem.bodySlot > -1 && slot == 1 || checkItem.legSlot > -1 && slot == 2))
          num = 1;
      }
      else if (context == 9)
      {
        if (checkItem.type == 0 || checkItem.headSlot > -1 && slot == 10 || (checkItem.bodySlot > -1 && slot == 11 || checkItem.legSlot > -1 && slot == 12))
          num = 1;
      }
      else if (context == 10)
      {
        if (checkItem.type == 0 || checkItem.accessory && !ItemSlot.AccCheck(checkItem, slot))
          num = 1;
      }
      else if (context == 11)
      {
        if (checkItem.type == 0 || checkItem.accessory && !ItemSlot.AccCheck(checkItem, slot))
          num = 1;
      }
      else if (context == 12)
        num = 2;
      else if (context == 15)
      {
        if (checkItem.type == 0 && inv[slot].type > 0)
        {
          if (player.BuyItem(inv[slot].GetStoreValue(), inv[slot].shopSpecialCurrency))
            num = 3;
        }
        else if (inv[slot].type == 0 && checkItem.type > 0 && (checkItem.type < 71 || checkItem.type > 74))
          num = 4;
      }
      else if (context == 16)
      {
        if (checkItem.type == 0 || Main.projHook[checkItem.shoot])
          num = 1;
      }
      else if (context == 17)
      {
        if (checkItem.type == 0 || checkItem.mountType != -1 && !MountID.Sets.Cart[checkItem.mountType])
          num = 1;
      }
      else if (context == 19)
      {
        if (checkItem.type == 0 || checkItem.buffType > 0 && Main.vanityPet[checkItem.buffType] && !Main.lightPet[checkItem.buffType])
          num = 1;
      }
      else if (context == 18)
      {
        if (checkItem.type == 0 || checkItem.mountType != -1 && MountID.Sets.Cart[checkItem.mountType])
          num = 1;
      }
      else if (context == 20 && (checkItem.type == 0 || checkItem.buffType > 0 && Main.lightPet[checkItem.buffType]))
        num = 1;
      return num;
    }

    public static void RightClick(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      ItemSlot.RightClick(ItemSlot.singleSlotArray, context, 0);
      inv = ItemSlot.singleSlotArray[0];
    }

    public static void RightClick(Item[] inv, int context = 0, int slot = 0)
    {
      Player player = Main.player[Main.myPlayer];
      inv[slot].newAndShiny = false;
      if (player.itemAnimation > 0)
        return;
      bool flag1 = false;
      if (context == 0)
      {
        flag1 = true;
        if (Main.mouseRight && (inv[slot].type >= 3318 && inv[slot].type <= 3332 || (inv[slot].type == 3860 || inv[slot].type == 3862) || inv[slot].type == 3861))
        {
          if (Main.mouseRightRelease)
          {
            player.OpenBossBag(inv[slot].type);
            --inv[slot].stack;
            if (inv[slot].stack == 0)
              inv[slot].SetDefaults(0, false);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            Recipe.FindRecipes();
          }
        }
        else if (Main.mouseRight && (inv[slot].type >= 2334 && inv[slot].type <= 2336 || inv[slot].type >= 3203 && inv[slot].type <= 3208))
        {
          if (Main.mouseRightRelease)
          {
            player.openCrate(inv[slot].type);
            --inv[slot].stack;
            if (inv[slot].stack == 0)
              inv[slot].SetDefaults(0, false);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            Recipe.FindRecipes();
          }
        }
        else if (Main.mouseRight && inv[slot].type == 3093)
        {
          if (Main.mouseRightRelease)
          {
            player.openHerbBag();
            --inv[slot].stack;
            if (inv[slot].stack == 0)
              inv[slot].SetDefaults(0, false);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            Recipe.FindRecipes();
          }
        }
        else if (Main.mouseRight && inv[slot].type == 1774)
        {
          if (Main.mouseRightRelease)
          {
            --inv[slot].stack;
            if (inv[slot].stack == 0)
              inv[slot].SetDefaults(0, false);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            player.openGoodieBag();
            Recipe.FindRecipes();
          }
        }
        else if (Main.mouseRight && inv[slot].type == 3085)
        {
          if (Main.mouseRightRelease && player.ConsumeItem(327, false))
          {
            --inv[slot].stack;
            if (inv[slot].stack == 0)
              inv[slot].SetDefaults(0, false);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            player.openLockBox();
            Recipe.FindRecipes();
          }
        }
        else if (Main.mouseRight && inv[slot].type == 1869)
        {
          if (Main.mouseRightRelease)
          {
            --inv[slot].stack;
            if (inv[slot].stack == 0)
              inv[slot].SetDefaults(0, false);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Main.stackSplit = 30;
            Main.mouseRightRelease = false;
            player.openPresent();
            Recipe.FindRecipes();
          }
        }
        else if (Main.mouseRight && Main.mouseRightRelease && (inv[slot].type == 599 || inv[slot].type == 600 || inv[slot].type == 601))
        {
          Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
          Main.stackSplit = 30;
          Main.mouseRightRelease = false;
          int num = Main.rand.Next(14);
          if (num == 0 && Main.hardMode)
            inv[slot].SetDefaults(602, false);
          else if (num <= 7)
          {
            inv[slot].SetDefaults(586, false);
            inv[slot].stack = Main.rand.Next(20, 50);
          }
          else
          {
            inv[slot].SetDefaults(591, false);
            inv[slot].stack = Main.rand.Next(20, 50);
          }
          Recipe.FindRecipes();
        }
        else
          flag1 = false;
      }
      else if (context == 9 || context == 11)
      {
        flag1 = true;
        if (Main.mouseRight && Main.mouseRightRelease && (inv[slot].type > 0 && inv[slot].stack > 0 || inv[slot - 10].type > 0 && inv[slot - 10].stack > 0))
        {
          bool flag2 = true;
          if (flag2 && context == 11 && (int) inv[slot].wingSlot > 0)
          {
            for (int index = 3; index < 10; ++index)
            {
              if ((int) inv[index].wingSlot > 0 && index != slot - 10)
                flag2 = false;
            }
          }
          if (flag2)
          {
            Utils.Swap<Item>(ref inv[slot], ref inv[slot - 10]);
            Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
            Recipe.FindRecipes();
            if (inv[slot].stack > 0)
            {
              if (context != 0)
              {
                if ((uint) (context - 8) <= 4U || (uint) (context - 16) <= 1U)
                  AchievementsHelper.HandleOnEquip(player, inv[slot], context);
              }
              else
                AchievementsHelper.NotifyItemPickup(player, inv[slot]);
            }
          }
        }
      }
      else if (context == 12)
      {
        flag1 = true;
        if (Main.mouseRight && Main.mouseRightRelease && (Main.mouseItem.stack < Main.mouseItem.maxStack && Main.mouseItem.type > 0) && (inv[slot].type > 0 && Main.mouseItem.type == inv[slot].type))
        {
          ++Main.mouseItem.stack;
          inv[slot].SetDefaults(0, false);
          Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
        }
      }
      else if (context == 15)
      {
        flag1 = true;
        Chest chest = Main.instance.shop[Main.npcShop];
        if (Main.stackSplit <= 1 && Main.mouseRight && inv[slot].type > 0 && (Main.mouseItem.IsTheSameAs(inv[slot]) || Main.mouseItem.type == 0))
        {
          int num1 = Main.superFastStack + 1;
          for (int index = 0; index < num1; ++index)
          {
            if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && (player.BuyItem(inv[slot].GetStoreValue(), inv[slot].shopSpecialCurrency) && inv[slot].stack > 0))
            {
              if (index == 0)
                Main.PlaySound(18, -1, -1, 1, 1f, 0.0f);
              if (Main.mouseItem.type == 0)
              {
                Main.mouseItem.netDefaults(inv[slot].netID);
                if ((int) inv[slot].prefix != 0)
                  Main.mouseItem.Prefix((int) inv[slot].prefix);
                Main.mouseItem.stack = 0;
              }
              ++Main.mouseItem.stack;
              Main.stackSplit = Main.stackSplit != 0 ? Main.stackDelay : 15;
              if (inv[slot].buyOnce)
              {
                Item obj = inv[slot];
                int num2 = obj.stack - 1;
                int num3 = num2;
                obj.stack = num3;
                if (num2 <= 0)
                  inv[slot].SetDefaults(0, false);
              }
            }
          }
        }
      }
      if (flag1)
        return;
      if ((context == 0 || context == 4 || context == 3) && (Main.mouseRight && Main.mouseRightRelease && inv[slot].maxStack == 1))
      {
        ItemSlot.SwapEquip(inv, context, slot);
      }
      else
      {
        if (Main.stackSplit > 1 || !Main.mouseRight)
          return;
        bool flag2 = true;
        if (context == 0 && inv[slot].maxStack <= 1)
          flag2 = false;
        if (context == 3 && inv[slot].maxStack <= 1)
          flag2 = false;
        if (context == 4 && inv[slot].maxStack <= 1)
          flag2 = false;
        if (!flag2 || !Main.mouseItem.IsTheSameAs(inv[slot]) && Main.mouseItem.type != 0 || Main.mouseItem.stack >= Main.mouseItem.maxStack && Main.mouseItem.type != 0)
          return;
        if (Main.mouseItem.type == 0)
        {
          Main.mouseItem = inv[slot].Clone();
          Main.mouseItem.stack = 0;
          Main.mouseItem.favorited = inv[slot].favorited && inv[slot].maxStack == 1;
        }
        ++Main.mouseItem.stack;
        --inv[slot].stack;
        if (inv[slot].stack <= 0)
          inv[slot] = new Item();
        Recipe.FindRecipes();
        Main.soundInstanceMenuTick.Stop();
        Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
        Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
        Main.stackSplit = Main.stackSplit != 0 ? Main.stackDelay : 15;
        if (context != 3 || Main.netMode != 1)
          return;
        NetMessage.SendData(32, -1, -1, (NetworkText) null, player.chest, (float) slot, 0.0f, 0.0f, 0, 0, 0);
      }
    }

    public static bool Equippable(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      int num = ItemSlot.Equippable(ItemSlot.singleSlotArray, context, 0) ? 1 : 0;
      inv = ItemSlot.singleSlotArray[0];
      return num != 0;
    }

    public static bool Equippable(Item[] inv, int context, int slot)
    {
      Player player = Main.player[Main.myPlayer];
      return (int) inv[slot].dye > 0 || Main.projHook[inv[slot].shoot] || inv[slot].mountType != -1 || (inv[slot].buffType > 0 && Main.lightPet[inv[slot].buffType] || inv[slot].buffType > 0 && Main.vanityPet[inv[slot].buffType]) || (inv[slot].headSlot >= 0 || inv[slot].bodySlot >= 0 || (inv[slot].legSlot >= 0 || inv[slot].accessory));
    }

    public static void SwapEquip(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      ItemSlot.SwapEquip(ItemSlot.singleSlotArray, context, 0);
      inv = ItemSlot.singleSlotArray[0];
    }

    public static void SwapEquip(Item[] inv, int context, int slot)
    {
      Player player = Main.player[Main.myPlayer];
      if ((int) inv[slot].dye > 0)
      {
        bool success;
        inv[slot] = ItemSlot.DyeSwap(inv[slot], out success);
        if (success)
        {
          Main.EquipPageSelected = 0;
          AchievementsHelper.HandleOnEquip(player, inv[slot], 12);
        }
      }
      else if (Main.projHook[inv[slot].shoot])
      {
        bool success;
        inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 4, out success);
        if (success)
        {
          Main.EquipPageSelected = 2;
          AchievementsHelper.HandleOnEquip(player, inv[slot], 16);
        }
      }
      else if (inv[slot].mountType != -1 && !MountID.Sets.Cart[inv[slot].mountType])
      {
        bool success;
        inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 3, out success);
        if (success)
        {
          Main.EquipPageSelected = 2;
          AchievementsHelper.HandleOnEquip(player, inv[slot], 17);
        }
      }
      else if (inv[slot].mountType != -1 && MountID.Sets.Cart[inv[slot].mountType])
      {
        bool success;
        inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 2, out success);
        if (success)
          Main.EquipPageSelected = 2;
      }
      else if (inv[slot].buffType > 0 && Main.lightPet[inv[slot].buffType])
      {
        bool success;
        inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 1, out success);
        if (success)
          Main.EquipPageSelected = 2;
      }
      else if (inv[slot].buffType > 0 && Main.vanityPet[inv[slot].buffType])
      {
        bool success;
        inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 0, out success);
        if (success)
          Main.EquipPageSelected = 2;
      }
      else
      {
        Item obj = inv[slot];
        bool success;
        inv[slot] = ItemSlot.ArmorSwap(inv[slot], out success);
        if (success)
        {
          Main.EquipPageSelected = 0;
          AchievementsHelper.HandleOnEquip(player, obj, obj.accessory ? 10 : 8);
        }
      }
      Recipe.FindRecipes();
      if (context != 3 || Main.netMode != 1)
        return;
      NetMessage.SendData(32, -1, -1, (NetworkText) null, player.chest, (float) slot, 0.0f, 0.0f, 0, 0, 0);
    }

    public static void Draw(SpriteBatch spriteBatch, ref Item inv, int context, Vector2 position, Color lightColor = null)
    {
      ItemSlot.singleSlotArray[0] = inv;
      ItemSlot.Draw(spriteBatch, ItemSlot.singleSlotArray, context, 0, position, lightColor);
      inv = ItemSlot.singleSlotArray[0];
    }

    public static void Draw(SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor = null)
    {
      Player player = Main.player[Main.myPlayer];
      Item obj = inv[slot];
      float inventoryScale = Main.inventoryScale;
      Color color1 = Color.get_White();
      if (Color.op_Inequality(lightColor, Color.get_Transparent()))
        color1 = lightColor;
      int ID = -1;
      bool flag1 = false;
      int num1 = 0;
      if (PlayerInput.UsingGamepadUI)
      {
        switch (context)
        {
          case 0:
          case 1:
          case 2:
            ID = slot;
            break;
          case 3:
          case 4:
            ID = 400 + slot;
            break;
          case 5:
            ID = 303;
            break;
          case 6:
            ID = 300;
            break;
          case 7:
            ID = 1500;
            break;
          case 8:
          case 9:
          case 10:
          case 11:
            ID = 100 + slot;
            break;
          case 12:
            if (inv == player.dye)
              ID = 120 + slot;
            if (inv == player.miscDyes)
            {
              ID = 185 + slot;
              break;
            }
            break;
          case 15:
            ID = 2700 + slot;
            break;
          case 16:
            ID = 184;
            break;
          case 17:
            ID = 183;
            break;
          case 18:
            ID = 182;
            break;
          case 19:
            ID = 180;
            break;
          case 20:
            ID = 181;
            break;
          case 22:
            if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig != -1)
              ID = 700 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig;
            if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall != -1)
            {
              ID = 1500 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall + 1;
              break;
            }
            break;
        }
        flag1 = UILinkPointNavigator.CurrentPoint == ID;
        if (context == 0)
        {
          num1 = player.DpadRadial.GetDrawMode(slot);
          if (num1 > 0 && !PlayerInput.CurrentProfile.UsingDpadHotbar())
            num1 = 0;
        }
      }
      Texture2D tex1 = Main.inventoryBackTexture;
      Color firstColor = Main.inventoryBack;
      bool flag2 = false;
      if (obj.type > 0 && obj.stack > 0 && (obj.favorited && context != 13) && (context != 21 && context != 22 && context != 14))
        tex1 = Main.inventoryBack10Texture;
      else if (obj.type > 0 && obj.stack > 0 && (ItemSlot.Options.HighlightNewItems && obj.newAndShiny) && (context != 13 && context != 21 && (context != 14 && context != 22)))
      {
        tex1 = Main.inventoryBack15Texture;
        float num2 = (float) ((double) ((float) Main.mouseTextColor / (float) byte.MaxValue) * 0.200000002980232 + 0.800000011920929);
        firstColor = firstColor.MultiplyRGBA(new Color(num2, num2, num2));
      }
      else if (PlayerInput.UsingGamepadUI && obj.type > 0 && (obj.stack > 0 && num1 != 0) && (context != 13 && context != 21 && context != 22))
      {
        tex1 = Main.inventoryBack15Texture;
        float num2 = (float) ((double) ((float) Main.mouseTextColor / (float) byte.MaxValue) * 0.200000002980232 + 0.800000011920929);
        firstColor = num1 != 1 ? firstColor.MultiplyRGBA(new Color(num2 / 2f, num2, num2 / 2f)) : firstColor.MultiplyRGBA(new Color(num2, num2 / 2f, num2 / 2f));
      }
      else if (context == 0 && slot < 10)
        tex1 = Main.inventoryBack9Texture;
      else if (context == 10 || context == 8 || (context == 16 || context == 17) || (context == 19 || context == 18 || context == 20))
        tex1 = Main.inventoryBack3Texture;
      else if (context == 11 || context == 9)
        tex1 = Main.inventoryBack8Texture;
      else if (context == 12)
        tex1 = Main.inventoryBack12Texture;
      else if (context == 3)
        tex1 = Main.inventoryBack5Texture;
      else if (context == 4)
        tex1 = Main.inventoryBack2Texture;
      else if (context == 7 || context == 5)
        tex1 = Main.inventoryBack4Texture;
      else if (context == 6)
        tex1 = Main.inventoryBack7Texture;
      else if (context == 13)
      {
        byte num2 = 200;
        if (slot == Main.player[Main.myPlayer].selectedItem)
        {
          tex1 = Main.inventoryBack14Texture;
          num2 = byte.MaxValue;
        }
        // ISSUE: explicit reference operation
        ((Color) @firstColor).\u002Ector((int) num2, (int) num2, (int) num2, (int) num2);
      }
      else if (context == 14 || context == 21)
        flag2 = true;
      else if (context == 15)
        tex1 = Main.inventoryBack6Texture;
      else if (context == 22)
        tex1 = Main.inventoryBack4Texture;
      if (context == 0 && ItemSlot.inventoryGlowTime[slot] > 0 && !inv[slot].favorited)
      {
        float num2 = Main.invAlpha / (float) byte.MaxValue;
        Color color2 = Color.op_Multiply(new Color(63, 65, 151, (int) byte.MaxValue), num2);
        Color color3 = Color.op_Multiply(Main.hslToRgb(ItemSlot.inventoryGlowHue[slot], 1f, 0.5f), num2);
        float num3 = (float) ItemSlot.inventoryGlowTime[slot] / 300f;
        float num4 = num3 * num3;
        Color color4 = color3;
        double num5 = (double) num4 / 2.0;
        firstColor = Color.Lerp(color2, color4, (float) num5);
        tex1 = Main.inventoryBack13Texture;
      }
      if ((context == 4 || context == 3) && (ItemSlot.inventoryGlowTimeChest[slot] > 0 && !inv[slot].favorited))
      {
        float num2 = Main.invAlpha / (float) byte.MaxValue;
        Color color2 = Color.op_Multiply(new Color(130, 62, 102, (int) byte.MaxValue), num2);
        if (context == 3)
          color2 = Color.op_Multiply(new Color(104, 52, 52, (int) byte.MaxValue), num2);
        Color color3 = Color.op_Multiply(Main.hslToRgb(ItemSlot.inventoryGlowHueChest[slot], 1f, 0.5f), num2);
        float num3 = (float) ItemSlot.inventoryGlowTimeChest[slot] / 300f;
        float num4 = num3 * num3;
        firstColor = Color.Lerp(color2, color3, num4 / 2f);
        tex1 = Main.inventoryBack13Texture;
      }
      if (flag1)
      {
        tex1 = Main.inventoryBack14Texture;
        firstColor = Color.get_White();
      }
      if (!flag2)
        spriteBatch.Draw(tex1, position, new Rectangle?(), firstColor, 0.0f, (Vector2) null, inventoryScale, (SpriteEffects) 0, 0.0f);
      int num6 = -1;
      switch (context)
      {
        case 8:
          if (slot == 0)
            num6 = 0;
          if (slot == 1)
            num6 = 6;
          if (slot == 2)
          {
            num6 = 12;
            break;
          }
          break;
        case 9:
          if (slot == 10)
            num6 = 3;
          if (slot == 11)
            num6 = 9;
          if (slot == 12)
          {
            num6 = 15;
            break;
          }
          break;
        case 10:
          num6 = 11;
          break;
        case 11:
          num6 = 2;
          break;
        case 12:
          num6 = 1;
          break;
        case 16:
          num6 = 4;
          break;
        case 17:
          num6 = 13;
          break;
        case 18:
          num6 = 7;
          break;
        case 19:
          num6 = 10;
          break;
        case 20:
          num6 = 17;
          break;
      }
      if ((obj.type <= 0 || obj.stack <= 0) && num6 != -1)
      {
        Texture2D tex2 = Main.extraTexture[54];
        Rectangle r = tex2.Frame(3, 6, num6 % 3, num6 / 3);
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        __Null& local1 = @r.Width;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        int num2 = ^(int&) local1 - 2;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) local1 = num2;
        // ISSUE: explicit reference operation
        // ISSUE: variable of a reference type
        __Null& local2 = @r.Height;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        int num3 = ^(int&) local2 - 2;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(int&) local2 = num3;
        spriteBatch.Draw(tex2, Vector2.op_Addition(position, Vector2.op_Multiply(Vector2.op_Division(tex1.Size(), 2f), inventoryScale)), new Rectangle?(r), Color.op_Multiply(Color.get_White(), 0.35f), 0.0f, Vector2.op_Division(r.Size(), 2f), inventoryScale, (SpriteEffects) 0, 0.0f);
      }
      Vector2 vector2_1 = Vector2.op_Multiply(tex1.Size(), inventoryScale);
      if (obj.type > 0 && obj.stack > 0)
      {
        Texture2D texture2D = Main.itemTexture[obj.type];
        Rectangle r = Main.itemAnimations[obj.type] == null ? texture2D.Frame(1, 1, 0, 0) : Main.itemAnimations[obj.type].GetFrame(texture2D);
        Color currentColor = color1;
        float scale = 1f;
        ItemSlot.GetItemLight(ref currentColor, ref scale, obj, false);
        float num2 = 1f;
        if (r.Width > 32 || r.Height > 32)
          num2 = r.Width <= r.Height ? 32f / (float) r.Height : 32f / (float) r.Width;
        float num3 = num2 * inventoryScale;
        Vector2 vector2_2 = Vector2.op_Subtraction(Vector2.op_Addition(position, Vector2.op_Division(vector2_1, 2f)), Vector2.op_Division(Vector2.op_Multiply(r.Size(), num3), 2f));
        Vector2 vector2_3 = Vector2.op_Multiply(r.Size(), (float) ((double) scale / 2.0 - 0.5));
        spriteBatch.Draw(texture2D, vector2_2, new Rectangle?(r), obj.GetAlpha(currentColor), 0.0f, vector2_3, num3 * scale, (SpriteEffects) 0, 0.0f);
        if (Color.op_Inequality(obj.color, Color.get_Transparent()))
          spriteBatch.Draw(texture2D, vector2_2, new Rectangle?(r), obj.GetColor(color1), 0.0f, vector2_3, num3 * scale, (SpriteEffects) 0, 0.0f);
        if (ItemID.Sets.TrapSigned[obj.type])
          spriteBatch.Draw(Main.wireTexture, Vector2.op_Addition(position, Vector2.op_Multiply(new Vector2(40f, 40f), inventoryScale)), new Rectangle?(new Rectangle(4, 58, 8, 8)), color1, 0.0f, new Vector2(4f), 1f, (SpriteEffects) 0, 0.0f);
        if (obj.stack > 1)
          ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, obj.stack.ToString(), Vector2.op_Addition(position, Vector2.op_Multiply(new Vector2(10f, 26f), inventoryScale)), color1, 0.0f, Vector2.get_Zero(), new Vector2(inventoryScale), -1f, inventoryScale);
        int num4 = -1;
        if (context == 13)
        {
          if (obj.DD2Summon)
          {
            for (int index = 0; index < 58; ++index)
            {
              if (inv[index].type == 3822)
                num4 += inv[index].stack;
            }
            if (num4 >= 0)
              ++num4;
          }
          if (obj.useAmmo > 0)
          {
            int useAmmo = obj.useAmmo;
            num4 = 0;
            for (int index = 0; index < 58; ++index)
            {
              if (inv[index].ammo == useAmmo)
                num4 += inv[index].stack;
            }
          }
          if (obj.fishingPole > 0)
          {
            num4 = 0;
            for (int index = 0; index < 58; ++index)
            {
              if (inv[index].bait > 0)
                num4 += inv[index].stack;
            }
          }
          if (obj.tileWand > 0)
          {
            int tileWand = obj.tileWand;
            num4 = 0;
            for (int index = 0; index < 58; ++index)
            {
              if (inv[index].type == tileWand)
                num4 += inv[index].stack;
            }
          }
          if (obj.type == 509 || obj.type == 851 || (obj.type == 850 || obj.type == 3612) || (obj.type == 3625 || obj.type == 3611))
          {
            num4 = 0;
            for (int index = 0; index < 58; ++index)
            {
              if (inv[index].type == 530)
                num4 += inv[index].stack;
            }
          }
        }
        if (num4 != -1)
          ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, num4.ToString(), Vector2.op_Addition(position, Vector2.op_Multiply(new Vector2(8f, 30f), inventoryScale)), color1, 0.0f, Vector2.get_Zero(), new Vector2(inventoryScale * 0.8f), -1f, inventoryScale);
        if (context == 13)
        {
          string text = string.Concat((object) (slot + 1));
          if (text == "10")
            text = "0";
          ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, Vector2.op_Addition(position, Vector2.op_Multiply(new Vector2(8f, 4f), inventoryScale)), color1, 0.0f, Vector2.get_Zero(), new Vector2(inventoryScale), -1f, inventoryScale);
        }
        if (context == 13 && obj.potion)
        {
          Vector2 vector2_4 = Vector2.op_Subtraction(Vector2.op_Addition(position, Vector2.op_Division(Vector2.op_Multiply(tex1.Size(), inventoryScale), 2f)), Vector2.op_Division(Vector2.op_Multiply(Main.cdTexture.Size(), inventoryScale), 2f));
          Color color2 = Color.op_Multiply(obj.GetAlpha(color1), (float) player.potionDelay / (float) player.potionDelayTime);
          spriteBatch.Draw(Main.cdTexture, vector2_4, new Rectangle?(), color2, 0.0f, (Vector2) null, num3, (SpriteEffects) 0, 0.0f);
        }
        if ((context == 10 || context == 18) && (obj.expertOnly && !Main.expertMode))
        {
          Vector2 vector2_4 = Vector2.op_Subtraction(Vector2.op_Addition(position, Vector2.op_Division(Vector2.op_Multiply(tex1.Size(), inventoryScale), 2f)), Vector2.op_Division(Vector2.op_Multiply(Main.cdTexture.Size(), inventoryScale), 2f));
          Color white = Color.get_White();
          spriteBatch.Draw(Main.cdTexture, vector2_4, new Rectangle?(), white, 0.0f, (Vector2) null, num3, (SpriteEffects) 0, 0.0f);
        }
      }
      else if (context == 6)
      {
        Texture2D trashTexture = Main.trashTexture;
        Vector2 vector2_2 = Vector2.op_Subtraction(Vector2.op_Addition(position, Vector2.op_Division(Vector2.op_Multiply(tex1.Size(), inventoryScale), 2f)), Vector2.op_Division(Vector2.op_Multiply(trashTexture.Size(), inventoryScale), 2f));
        spriteBatch.Draw(trashTexture, vector2_2, new Rectangle?(), new Color(100, 100, 100, 100), 0.0f, (Vector2) null, inventoryScale, (SpriteEffects) 0, 0.0f);
      }
      if (context == 0 && slot < 10)
      {
        float num2 = inventoryScale;
        string text = string.Concat((object) (slot + 1));
        if (text == "10")
          text = "0";
        Color inventoryBack = Main.inventoryBack;
        int num3 = 0;
        if (Main.player[Main.myPlayer].selectedItem == slot)
        {
          num3 -= 3;
          // ISSUE: explicit reference operation
          ((Color) @inventoryBack).set_R(byte.MaxValue);
          // ISSUE: explicit reference operation
          ((Color) @inventoryBack).set_B((byte) 0);
          // ISSUE: explicit reference operation
          ((Color) @inventoryBack).set_G((byte) 210);
          // ISSUE: explicit reference operation
          ((Color) @inventoryBack).set_A((byte) 100);
          float num4 = num2 * 1.4f;
        }
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, Vector2.op_Addition(position, Vector2.op_Multiply(new Vector2(6f, (float) (4 + num3)), inventoryScale)), inventoryBack, 0.0f, Vector2.get_Zero(), new Vector2(inventoryScale), -1f, inventoryScale);
      }
      if (ID == -1)
        return;
      UILinkPointNavigator.SetPosition(ID, Vector2.op_Addition(position, Vector2.op_Multiply(vector2_1, 0.75f)));
    }

    public static void MouseHover(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      ItemSlot.MouseHover(ItemSlot.singleSlotArray, context, 0);
      inv = ItemSlot.singleSlotArray[0];
    }

    public static void MouseHover(Item[] inv, int context = 0, int slot = 0)
    {
      if (context == 6 && Main.hoverItemName == null)
        Main.hoverItemName = Lang.inter[3].Value;
      if (inv[slot].type > 0 && inv[slot].stack > 0)
      {
        ItemSlot._customCurrencyForSavings = inv[slot].shopSpecialCurrency;
        Main.hoverItemName = inv[slot].Name;
        if (inv[slot].stack > 1)
          Main.hoverItemName = Main.hoverItemName + " (" + (object) inv[slot].stack + ")";
        Main.HoverItem = inv[slot].Clone();
        if (context == 8 && slot <= 2)
          Main.HoverItem.wornArmor = true;
        if (context == 11 || context == 9)
          Main.HoverItem.social = true;
        if (context != 15)
          return;
        Main.HoverItem.buy = true;
      }
      else
      {
        if (context == 10 || context == 11)
          Main.hoverItemName = Lang.inter[9].Value;
        if (context == 11)
          Main.hoverItemName = Lang.inter[11].Value + " " + Main.hoverItemName;
        if (context == 8 || context == 9)
        {
          if (slot == 0 || slot == 10)
            Main.hoverItemName = Lang.inter[12].Value;
          if (slot == 1 || slot == 11)
            Main.hoverItemName = Lang.inter[13].Value;
          if (slot == 2 || slot == 12)
            Main.hoverItemName = Lang.inter[14].Value;
          if (slot >= 10)
            Main.hoverItemName = Lang.inter[11].Value + " " + Main.hoverItemName;
        }
        if (context == 12)
          Main.hoverItemName = Lang.inter[57].Value;
        if (context == 16)
          Main.hoverItemName = Lang.inter[90].Value;
        if (context == 17)
          Main.hoverItemName = Lang.inter[91].Value;
        if (context == 19)
          Main.hoverItemName = Lang.inter[92].Value;
        if (context == 18)
          Main.hoverItemName = Lang.inter[93].Value;
        if (context != 20)
          return;
        Main.hoverItemName = Lang.inter[94].Value;
      }
    }

    private static bool AccCheck(Item item, int slot)
    {
      Player player = Main.player[Main.myPlayer];
      if (slot != -1 && (player.armor[slot].IsTheSameAs(item) || (int) player.armor[slot].wingSlot > 0 && (int) item.wingSlot > 0))
        return false;
      for (int index = 0; index < player.armor.Length; ++index)
      {
        if (slot < 10 && index < 10 && ((int) item.wingSlot > 0 && (int) player.armor[index].wingSlot > 0 || slot >= 10 && index >= 10 && ((int) item.wingSlot > 0 && (int) player.armor[index].wingSlot > 0)) || item.IsTheSameAs(player.armor[index]))
          return true;
      }
      return false;
    }

    private static Item DyeSwap(Item item, out bool success)
    {
      success = false;
      if ((int) item.dye <= 0)
        return item;
      Player player = Main.player[Main.myPlayer];
      for (int index = 0; index < 10; ++index)
      {
        if (player.dye[index].type == 0)
        {
          ItemSlot.dyeSlotCount = index;
          break;
        }
      }
      if (ItemSlot.dyeSlotCount >= 10)
        ItemSlot.dyeSlotCount = 0;
      if (ItemSlot.dyeSlotCount < 0)
        ItemSlot.dyeSlotCount = 9;
      Item obj = player.dye[ItemSlot.dyeSlotCount].Clone();
      player.dye[ItemSlot.dyeSlotCount] = item.Clone();
      ++ItemSlot.dyeSlotCount;
      if (ItemSlot.dyeSlotCount >= 10)
        ItemSlot.accSlotCount = 0;
      Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
      Recipe.FindRecipes();
      success = true;
      return obj;
    }

    private static Item ArmorSwap(Item item, out bool success)
    {
      success = false;
      if (item.headSlot == -1 && item.bodySlot == -1 && (item.legSlot == -1 && !item.accessory))
        return item;
      Player player = Main.player[Main.myPlayer];
      int index1 = !item.vanity || item.accessory ? 0 : 10;
      item.favorited = false;
      Item obj = item;
      if (item.headSlot != -1)
      {
        obj = player.armor[index1].Clone();
        player.armor[index1] = item.Clone();
      }
      else if (item.bodySlot != -1)
      {
        obj = player.armor[index1 + 1].Clone();
        player.armor[index1 + 1] = item.Clone();
      }
      else if (item.legSlot != -1)
      {
        obj = player.armor[index1 + 2].Clone();
        player.armor[index1 + 2] = item.Clone();
      }
      else if (item.accessory)
      {
        int num = 5 + Main.player[Main.myPlayer].extraAccessorySlots;
        for (int index2 = 3; index2 < 3 + num; ++index2)
        {
          if (player.armor[index2].type == 0)
          {
            ItemSlot.accSlotCount = index2 - 3;
            break;
          }
        }
        for (int index2 = 0; index2 < player.armor.Length; ++index2)
        {
          if (item.IsTheSameAs(player.armor[index2]))
            ItemSlot.accSlotCount = index2 - 3;
          if (index2 < 10 && (int) item.wingSlot > 0 && (int) player.armor[index2].wingSlot > 0)
            ItemSlot.accSlotCount = index2 - 3;
        }
        if (ItemSlot.accSlotCount >= num)
          ItemSlot.accSlotCount = 0;
        if (ItemSlot.accSlotCount < 0)
          ItemSlot.accSlotCount = num - 1;
        int index3 = 3 + ItemSlot.accSlotCount;
        for (int index2 = 0; index2 < player.armor.Length; ++index2)
        {
          if (item.IsTheSameAs(player.armor[index2]))
            index3 = index2;
        }
        obj = player.armor[index3].Clone();
        player.armor[index3] = item.Clone();
        ++ItemSlot.accSlotCount;
        if (ItemSlot.accSlotCount >= num)
          ItemSlot.accSlotCount = 0;
      }
      Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
      Recipe.FindRecipes();
      success = true;
      return obj;
    }

    private static Item EquipSwap(Item item, Item[] inv, int slot, out bool success)
    {
      success = false;
      Player player = Main.player[Main.myPlayer];
      item.favorited = false;
      Item obj = inv[slot].Clone();
      inv[slot] = item.Clone();
      Main.PlaySound(7, -1, -1, 1, 1f, 0.0f);
      Recipe.FindRecipes();
      success = true;
      return obj;
    }

    public static void EquipPage(Item item)
    {
      Main.EquipPage = -1;
      if (Main.projHook[item.shoot])
        Main.EquipPage = 2;
      else if (item.mountType != -1)
        Main.EquipPage = 2;
      else if (item.buffType > 0 && Main.vanityPet[item.buffType])
        Main.EquipPage = 2;
      else if (item.buffType > 0 && Main.lightPet[item.buffType])
        Main.EquipPage = 2;
      else if ((int) item.dye > 0 && Main.EquipPageSelected == 1)
      {
        Main.EquipPage = 0;
      }
      else
      {
        if (item.legSlot == -1 && item.headSlot == -1 && (item.bodySlot == -1 && !item.accessory))
          return;
        Main.EquipPage = 0;
      }
    }

    public static void DrawMoney(SpriteBatch sb, string text, float shopx, float shopy, int[] coinsArray, bool horizontal = false)
    {
      Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text, shopx, shopy + 40f, Color.op_Multiply(Color.get_White(), (float) Main.mouseTextColor / (float) byte.MaxValue), Color.get_Black(), Vector2.get_Zero(), 1f);
      if (horizontal)
      {
        for (int index = 0; index < 4; ++index)
        {
          if (index == 0)
          {
            int coins = coinsArray[3 - index];
          }
          Vector2 vector2;
          // ISSUE: explicit reference operation
          ((Vector2) @vector2).\u002Ector((float) ((double) shopx + ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.get_One(), -1f).X + (double) (24 * index) + 45.0), shopy + 50f);
          sb.Draw(Main.itemTexture[74 - index], vector2, new Rectangle?(), Color.get_White(), 0.0f, Vector2.op_Division(Main.itemTexture[74 - index].Size(), 2f), 1f, (SpriteEffects) 0, 0.0f);
          Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, coinsArray[3 - index].ToString(), (float) (vector2.X - 11.0), (float) vector2.Y, Color.get_White(), Color.get_Black(), new Vector2(0.3f), 0.75f);
        }
      }
      else
      {
        for (int index = 0; index < 4; ++index)
        {
          int num = index != 0 || coinsArray[3 - index] <= 99 ? 0 : -6;
          sb.Draw(Main.itemTexture[74 - index], new Vector2(shopx + 11f + (float) (24 * index), shopy + 75f), new Rectangle?(), Color.get_White(), 0.0f, Vector2.op_Division(Main.itemTexture[74 - index].Size(), 2f), 1f, (SpriteEffects) 0, 0.0f);
          Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, coinsArray[3 - index].ToString(), shopx + (float) (24 * index) + (float) num, shopy + 75f, Color.get_White(), Color.get_Black(), new Vector2(0.3f), 0.75f);
        }
      }
    }

    public static void DrawSavings(SpriteBatch sb, float shopx, float shopy, bool horizontal = false)
    {
      Player player = Main.player[Main.myPlayer];
      if (ItemSlot._customCurrencyForSavings != -1)
      {
        CustomCurrencyManager.DrawSavings(sb, ItemSlot._customCurrencyForSavings, shopx, shopy, horizontal);
      }
      else
      {
        bool overFlowing;
        long num1 = Utils.CoinsCount(out overFlowing, player.bank.item);
        long num2 = Utils.CoinsCount(out overFlowing, player.bank2.item);
        long num3 = Utils.CoinsCount(out overFlowing, player.bank3.item);
        long count = Utils.CoinsCombineStacks(out overFlowing, num1, num2, num3);
        if (count <= 0L)
          return;
        if (num3 > 0L)
          sb.Draw(Main.itemTexture[3813], Utils.CenteredRectangle(new Vector2(shopx + 92f, shopy + 45f), Vector2.op_Multiply(Main.itemTexture[3813].Size(), 0.65f)), new Rectangle?(), Color.get_White());
        if (num2 > 0L)
          sb.Draw(Main.itemTexture[346], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Vector2.op_Multiply(Main.itemTexture[346].Size(), 0.65f)), new Rectangle?(), Color.get_White());
        if (num1 > 0L)
          sb.Draw(Main.itemTexture[87], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 60f), Vector2.op_Multiply(Main.itemTexture[87].Size(), 0.65f)), new Rectangle?(), Color.get_White());
        ItemSlot.DrawMoney(sb, Lang.inter[66].Value, shopx, shopy, Utils.CoinsSplit(count), horizontal);
      }
    }

    public static void DrawRadialDpad(SpriteBatch sb, Vector2 position)
    {
      if (!PlayerInput.UsingGamepad || !PlayerInput.CurrentProfile.UsingDpadHotbar())
        return;
      Player player = Main.player[Main.myPlayer];
      if (player.chest != -1)
        return;
      Texture2D tex = Main.hotbarRadialTexture[0];
      float num = (float) Main.mouseTextColor / (float) byte.MaxValue;
      Color color = Color.op_Multiply(Color.get_White(), (float) (1.0 - (1.0 - (double) num) * (1.0 - (double) num)) * 0.785f);
      sb.Draw(tex, position, new Rectangle?(), color, 0.0f, Vector2.op_Division(tex.Size(), 2f), Main.inventoryScale, (SpriteEffects) 0, 0.0f);
      for (int index = 0; index < 4; ++index)
      {
        int binding = player.DpadRadial.Bindings[index];
        if (binding != -1)
          ItemSlot.Draw(sb, player.inventory, 14, binding, Vector2.op_Addition(Vector2.op_Addition(position, new Vector2((float) (tex.get_Width() / 3), 0.0f).RotatedBy(1.57079637050629 * (double) index - 1.57079637050629, (Vector2) null)), new Vector2(-26f * Main.inventoryScale)), Color.get_White());
      }
    }

    public static void DrawRadialCircular(SpriteBatch sb, Vector2 position)
    {
      ItemSlot.CircularRadialOpacity = MathHelper.Clamp(ItemSlot.CircularRadialOpacity + (!PlayerInput.UsingGamepad || !PlayerInput.Triggers.Current.RadialHotbar ? -0.15f : 0.25f), 0.0f, 1f);
      if ((double) ItemSlot.CircularRadialOpacity == 0.0)
        return;
      Player player = Main.player[Main.myPlayer];
      Texture2D texture2D = Main.hotbarRadialTexture[2];
      float num1 = ItemSlot.CircularRadialOpacity * 0.9f;
      float num2 = ItemSlot.CircularRadialOpacity * 1f;
      float num3 = (float) Main.mouseTextColor / (float) byte.MaxValue;
      Color color = Color.op_Multiply(Color.op_Multiply(Color.get_White(), (float) (1.0 - (1.0 - (double) num3) * (1.0 - (double) num3)) * 0.785f), num1);
      Texture2D tex = Main.hotbarRadialTexture[1];
      float num4 = 6.283185f / (float) player.CircularRadial.RadialCount;
      float num5 = -1.570796f;
      for (int index = 0; index < player.CircularRadial.RadialCount; ++index)
      {
        int binding = player.CircularRadial.Bindings[index];
        Vector2 vector2 = Vector2.op_Multiply(new Vector2(150f, 0.0f).RotatedBy((double) num5 + (double) num4 * (double) index, (Vector2) null), num2);
        float num6 = 0.85f;
        if (player.CircularRadial.SelectedBinding == index)
          num6 = 1.7f;
        sb.Draw(tex, Vector2.op_Addition(position, vector2), new Rectangle?(), Color.op_Multiply(color, num6), 0.0f, Vector2.op_Division(tex.Size(), 2f), num2 * num6, (SpriteEffects) 0, 0.0f);
        if (binding != -1)
        {
          double inventoryScale = (double) Main.inventoryScale;
          Main.inventoryScale = num2 * num6;
          ItemSlot.Draw(sb, player.inventory, 14, binding, Vector2.op_Addition(Vector2.op_Addition(position, vector2), new Vector2(-26f * num2 * num6)), Color.get_White());
          Main.inventoryScale = (float) inventoryScale;
        }
      }
    }

    public static void DrawRadialQuicks(SpriteBatch sb, Vector2 position)
    {
      ItemSlot.QuicksRadialOpacity = MathHelper.Clamp(ItemSlot.QuicksRadialOpacity + (!PlayerInput.UsingGamepad || !PlayerInput.Triggers.Current.RadialQuickbar ? -0.15f : 0.25f), 0.0f, 1f);
      if ((double) ItemSlot.QuicksRadialOpacity == 0.0)
        return;
      Player player = Main.player[Main.myPlayer];
      Texture2D tex = Main.hotbarRadialTexture[2];
      Texture2D quicksIconTexture = Main.quicksIconTexture;
      float num1 = ItemSlot.QuicksRadialOpacity * 0.9f;
      float num2 = ItemSlot.QuicksRadialOpacity * 1f;
      float num3 = (float) Main.mouseTextColor / (float) byte.MaxValue;
      Color color = Color.op_Multiply(Color.op_Multiply(Color.get_White(), (float) (1.0 - (1.0 - (double) num3) * (1.0 - (double) num3)) * 0.785f), num1);
      float num4 = 6.283185f / (float) player.QuicksRadial.RadialCount;
      float num5 = -1.570796f;
      Item obj1 = player.QuickHeal_GetItemToUse();
      Item obj2 = player.QuickMana_GetItemToUse();
      Item obj3 = (Item) null;
      if (obj1 == null)
      {
        obj1 = new Item();
        obj1.SetDefaults(28, false);
      }
      if (obj2 == null)
      {
        obj2 = new Item();
        obj2.SetDefaults(110, false);
      }
      if (obj3 == null)
      {
        obj3 = new Item();
        obj3.SetDefaults(292, false);
      }
      for (int index = 0; index < player.QuicksRadial.RadialCount; ++index)
      {
        Item inv = obj1;
        if (index == 1)
          inv = obj3;
        if (index == 2)
          inv = obj2;
        int binding = player.QuicksRadial.Bindings[index];
        Vector2 vector2 = Vector2.op_Multiply(new Vector2(120f, 0.0f).RotatedBy((double) num5 + (double) num4 * (double) index, (Vector2) null), num2);
        float num6 = 0.85f;
        if (player.QuicksRadial.SelectedBinding == index)
          num6 = 1.7f;
        sb.Draw(tex, Vector2.op_Addition(position, vector2), new Rectangle?(), Color.op_Multiply(color, num6), 0.0f, Vector2.op_Division(tex.Size(), 2f), (float) ((double) num2 * (double) num6 * 1.29999995231628), (SpriteEffects) 0, 0.0f);
        double inventoryScale = (double) Main.inventoryScale;
        Main.inventoryScale = num2 * num6;
        ItemSlot.Draw(sb, ref inv, 14, Vector2.op_Addition(Vector2.op_Addition(position, vector2), new Vector2(-26f * num2 * num6)), Color.get_White());
        Main.inventoryScale = (float) inventoryScale;
        sb.Draw(quicksIconTexture, Vector2.op_Addition(Vector2.op_Addition(position, vector2), Vector2.op_Multiply(Vector2.op_Multiply(Vector2.op_Multiply(new Vector2(34f, 20f), 0.85f), num2), num6)), new Rectangle?(), Color.op_Multiply(color, num6), 0.0f, Vector2.op_Division(tex.Size(), 2f), (float) ((double) num2 * (double) num6 * 1.29999995231628), (SpriteEffects) 0, 0.0f);
      }
    }

    public static void GetItemLight(ref Color currentColor, Item item, bool outInTheWorld = false)
    {
      float scale = 1f;
      ItemSlot.GetItemLight(ref currentColor, ref scale, item, outInTheWorld);
    }

    public static void GetItemLight(ref Color currentColor, int type, bool outInTheWorld = false)
    {
      float scale = 1f;
      ItemSlot.GetItemLight(ref currentColor, ref scale, type, outInTheWorld);
    }

    public static void GetItemLight(ref Color currentColor, ref float scale, Item item, bool outInTheWorld = false)
    {
      ItemSlot.GetItemLight(ref currentColor, ref scale, item.type, outInTheWorld);
    }

    public static Color GetItemLight(ref Color currentColor, ref float scale, int type, bool outInTheWorld = false)
    {
      if (type < 0 || type > 3930)
        return currentColor;
      if (type == 662 || type == 663)
      {
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_R((byte) Main.DiscoR);
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_G((byte) Main.DiscoG);
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_B((byte) Main.DiscoB);
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_A(byte.MaxValue);
      }
      else if (ItemID.Sets.ItemIconPulse[type])
      {
        scale = Main.essScale;
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_R((byte) ((double) ((Color) @currentColor).get_R() * (double) scale));
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_G((byte) ((double) ((Color) @currentColor).get_G() * (double) scale));
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_B((byte) ((double) ((Color) @currentColor).get_B() * (double) scale));
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_A((byte) ((double) ((Color) @currentColor).get_A() * (double) scale));
      }
      else if (type == 58 || type == 184)
      {
        scale = (float) ((double) Main.essScale * 0.25 + 0.75);
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_R((byte) ((double) ((Color) @currentColor).get_R() * (double) scale));
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_G((byte) ((double) ((Color) @currentColor).get_G() * (double) scale));
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_B((byte) ((double) ((Color) @currentColor).get_B() * (double) scale));
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        ((Color) @currentColor).set_A((byte) ((double) ((Color) @currentColor).get_A() * (double) scale));
      }
      return currentColor;
    }

    public static string GetGamepadInstructions(ref Item inv, int context = 0)
    {
      ItemSlot.singleSlotArray[0] = inv;
      string gamepadInstructions = ItemSlot.GetGamepadInstructions(ItemSlot.singleSlotArray, context, 0);
      inv = ItemSlot.singleSlotArray[0];
      return gamepadInstructions;
    }

    public static string GetGamepadInstructions(Item[] inv, int context = 0, int slot = 0)
    {
      Player player = Main.player[Main.myPlayer];
      string str1 = "";
      if (inv == null || inv[slot] == null || Main.mouseItem == null)
        return str1;
      if (context == 0 || context == 1 || context == 2)
      {
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          string str2;
          if (Main.mouseItem.type > 0)
          {
            str2 = str1 + PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
            if (inv[slot].type == Main.mouseItem.type && Main.mouseItem.stack < inv[slot].maxStack && inv[slot].maxStack > 1)
              str2 += PlayerInput.BuildCommand(Lang.misc[55].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
          }
          else
          {
            if (context == 0 && player.chest == -1)
              player.DpadRadial.ChangeBinding(slot);
            str2 = str1 + PlayerInput.BuildCommand(Lang.misc[54].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
            if (inv[slot].maxStack > 1)
              str2 += PlayerInput.BuildCommand(Lang.misc[55].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
          }
          if (inv[slot].maxStack == 1 && ItemSlot.Equippable(inv, context, slot))
          {
            str2 += PlayerInput.BuildCommand(Lang.misc[67].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
            if (PlayerInput.Triggers.JustPressed.Grapple)
              ItemSlot.SwapEquip(inv, context, slot);
          }
          str1 = str2 + PlayerInput.BuildCommand(Lang.misc[83].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartCursor"]);
          if (PlayerInput.Triggers.JustPressed.SmartCursor)
            inv[slot].favorited = !inv[slot].favorited;
        }
        else if (Main.mouseItem.type > 0)
          str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
      }
      if (context == 3 || context == 4)
      {
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          if (Main.mouseItem.type > 0)
          {
            str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
            if (inv[slot].type == Main.mouseItem.type && Main.mouseItem.stack < inv[slot].maxStack && inv[slot].maxStack > 1)
              str1 += PlayerInput.BuildCommand(Lang.misc[55].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
          }
          else
          {
            str1 += PlayerInput.BuildCommand(Lang.misc[54].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
            if (inv[slot].maxStack > 1)
              str1 += PlayerInput.BuildCommand(Lang.misc[55].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
          }
          if (inv[slot].maxStack == 1 && ItemSlot.Equippable(inv, context, slot))
          {
            str1 += PlayerInput.BuildCommand(Lang.misc[67].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
            if (PlayerInput.Triggers.JustPressed.Grapple)
              ItemSlot.SwapEquip(inv, context, slot);
          }
        }
        else if (Main.mouseItem.type > 0)
          str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
      }
      if (context == 15)
      {
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          if (Main.mouseItem.type > 0)
          {
            if (inv[slot].type == Main.mouseItem.type && Main.mouseItem.stack < inv[slot].maxStack && inv[slot].maxStack > 1)
              str1 += PlayerInput.BuildCommand(Lang.misc[91].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
          }
          else
            str1 += PlayerInput.BuildCommand(Lang.misc[90].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"], PlayerInput.ProfileGamepadUI.KeyStatus["MouseRight"]);
        }
        else if (Main.mouseItem.type > 0)
          str1 += PlayerInput.BuildCommand(Lang.misc[92].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
      }
      if (context == 8 || context == 9 || (context == 16 || context == 17) || (context == 18 || context == 19 || context == 20))
      {
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          if (Main.mouseItem.type > 0)
          {
            if (ItemSlot.Equippable(ref Main.mouseItem, context))
              str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          }
          else
            str1 += PlayerInput.BuildCommand(Lang.misc[54].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          if (context == 8 && slot >= 3)
          {
            bool flag = player.hideVisual[slot];
            str1 += PlayerInput.BuildCommand(Lang.misc[flag ? 77 : 78].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
            if (PlayerInput.Triggers.JustPressed.Grapple)
            {
              player.hideVisual[slot] = !player.hideVisual[slot];
              Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
              if (Main.netMode == 1)
                NetMessage.SendData(4, -1, -1, (NetworkText) null, Main.myPlayer, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }
          }
          if ((context == 16 || context == 17 || (context == 18 || context == 19) || context == 20) && slot < 2)
          {
            bool flag = player.hideMisc[slot];
            str1 += PlayerInput.BuildCommand(Lang.misc[flag ? 77 : 78].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
            if (PlayerInput.Triggers.JustPressed.Grapple)
            {
              player.hideMisc[slot] = !player.hideMisc[slot];
              Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
              if (Main.netMode == 1)
                NetMessage.SendData(4, -1, -1, (NetworkText) null, Main.myPlayer, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }
          }
        }
        else
        {
          if (Main.mouseItem.type > 0 && ItemSlot.Equippable(ref Main.mouseItem, context))
            str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          if (context == 8 && slot >= 3)
          {
            bool flag = player.hideVisual[slot];
            str1 += PlayerInput.BuildCommand(Lang.misc[flag ? 77 : 78].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
            if (PlayerInput.Triggers.JustPressed.Grapple)
            {
              player.hideVisual[slot] = !player.hideVisual[slot];
              Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
              if (Main.netMode == 1)
                NetMessage.SendData(4, -1, -1, (NetworkText) null, Main.myPlayer, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }
          }
          if ((context == 16 || context == 17 || (context == 18 || context == 19) || context == 20) && slot < 2)
          {
            bool flag = player.hideMisc[slot];
            str1 += PlayerInput.BuildCommand(Lang.misc[flag ? 77 : 78].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
            if (PlayerInput.Triggers.JustPressed.Grapple)
            {
              if (slot == 0)
                player.TogglePet();
              if (slot == 1)
                player.ToggleLight();
              Main.mouseLeftRelease = false;
              Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
              if (Main.netMode == 1)
                NetMessage.SendData(4, -1, -1, (NetworkText) null, Main.myPlayer, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            }
          }
        }
      }
      if (context == 12)
      {
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          if (Main.mouseItem.type > 0)
          {
            if ((int) Main.mouseItem.dye > 0)
              str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          }
          else
            str1 += PlayerInput.BuildCommand(Lang.misc[54].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          if (context == 12)
          {
            int num = -1;
            if (inv == player.dye)
              num = slot;
            if (inv == player.miscDyes)
              num = 10 + slot;
            if (num != -1)
            {
              if (num < 10)
              {
                bool flag = player.hideVisual[slot];
                str1 += PlayerInput.BuildCommand(Lang.misc[flag ? 77 : 78].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
                if (PlayerInput.Triggers.JustPressed.Grapple)
                {
                  player.hideVisual[slot] = !player.hideVisual[slot];
                  Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
                  if (Main.netMode == 1)
                    NetMessage.SendData(4, -1, -1, (NetworkText) null, Main.myPlayer, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                }
              }
              else
              {
                bool flag = player.hideMisc[slot];
                str1 += PlayerInput.BuildCommand(Lang.misc[flag ? 77 : 78].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["Grapple"]);
                if (PlayerInput.Triggers.JustPressed.Grapple)
                {
                  player.hideMisc[slot] = !player.hideMisc[slot];
                  Main.PlaySound(12, -1, -1, 1, 1f, 0.0f);
                  if (Main.netMode == 1)
                    NetMessage.SendData(4, -1, -1, (NetworkText) null, Main.myPlayer, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                }
              }
            }
          }
        }
        else if (Main.mouseItem.type > 0 && (int) Main.mouseItem.dye > 0)
          str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
        return str1;
      }
      if (context == 6)
      {
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          if (Main.mouseItem.type > 0)
            str1 += PlayerInput.BuildCommand(Lang.misc[74].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          else
            str1 += PlayerInput.BuildCommand(Lang.misc[54].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
        }
        else if (Main.mouseItem.type > 0)
          str1 += PlayerInput.BuildCommand(Lang.misc[74].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
        return str1;
      }
      if (context == 5 || context == 7)
      {
        bool flag = false;
        if (context == 5)
          flag = Main.mouseItem.Prefix(-3) || Main.mouseItem.type == 0;
        if (context == 7)
          flag = Main.mouseItem.material;
        if (inv[slot].type > 0 && inv[slot].stack > 0)
        {
          if (Main.mouseItem.type > 0)
          {
            if (flag)
              str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
          }
          else
            str1 += PlayerInput.BuildCommand(Lang.misc[54].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
        }
        else if (Main.mouseItem.type > 0 & flag)
          str1 += PlayerInput.BuildCommand(Lang.misc[65].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["MouseLeft"]);
        return str1;
      }
      string overrideInstructions = ItemSlot.GetOverrideInstructions(inv, context, slot);
      if ((Main.mouseItem.type <= 0 ? 0 : (context == 0 || context == 1 || (context == 2 || context == 6) || context == 15 ? 1 : (context == 7 ? 1 : 0))) != 0 && string.IsNullOrEmpty(overrideInstructions))
      {
        str1 += PlayerInput.BuildCommand(Lang.inter[121].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartSelect"]);
        if (PlayerInput.Triggers.JustPressed.SmartSelect)
          player.DropSelectedItem();
      }
      else if (!string.IsNullOrEmpty(overrideInstructions))
      {
        ItemSlot.ShiftForcedOn = true;
        int cursorOverride = Main.cursorOverride;
        ItemSlot.OverrideHover(inv, context, slot);
        if (-1 != Main.cursorOverride)
        {
          str1 += PlayerInput.BuildCommand(overrideInstructions, false, PlayerInput.ProfileGamepadUI.KeyStatus["SmartSelect"]);
          if (PlayerInput.Triggers.JustPressed.SmartSelect)
            ItemSlot.LeftClick(inv, context, slot);
        }
        Main.cursorOverride = cursorOverride;
        ItemSlot.ShiftForcedOn = false;
      }
      int num1 = 0;
      if (ItemSlot.IsABuildingItem(Main.mouseItem))
        num1 = 1;
      if (num1 == 0 && Main.mouseItem.stack <= 0 && (context == 0 && ItemSlot.IsABuildingItem(inv[slot])))
        num1 = 2;
      if (Main.autoPause)
        num1 = 0;
      if (num1 > 0)
      {
        Item mouseItem = Main.mouseItem;
        if (num1 == 1)
          mouseItem = Main.mouseItem;
        if (num1 == 2)
          mouseItem = inv[slot];
        if (num1 != 1 || player.ItemSpace(mouseItem))
        {
          if (mouseItem.damage > 0 && mouseItem.ammo == 0)
            str1 += PlayerInput.BuildCommand(Lang.misc[60].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["QuickMount"]);
          else if (mouseItem.createTile >= 0 || mouseItem.createWall > 0)
            str1 += PlayerInput.BuildCommand(Lang.misc[61].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["QuickMount"]);
          else
            str1 += PlayerInput.BuildCommand(Lang.misc[63].Value, false, PlayerInput.ProfileGamepadUI.KeyStatus["QuickMount"]);
        }
        if (PlayerInput.Triggers.JustPressed.QuickMount)
          PlayerInput.EnterBuildingMode();
      }
      return str1;
    }

    public static bool IsABuildingItem(Item item)
    {
      if (item.type > 0 && item.stack > 0 && item.useStyle > 0)
        return item.useTime > 0;
      return false;
    }

    public class Options
    {
      public static bool DisableLeftShiftTrashCan = false;
      public static bool HighlightNewItems = true;
    }

    public class Context
    {
      public const int InventoryItem = 0;
      public const int InventoryCoin = 1;
      public const int InventoryAmmo = 2;
      public const int ChestItem = 3;
      public const int BankItem = 4;
      public const int PrefixItem = 5;
      public const int TrashItem = 6;
      public const int GuideItem = 7;
      public const int EquipArmor = 8;
      public const int EquipArmorVanity = 9;
      public const int EquipAccessory = 10;
      public const int EquipAccessoryVanity = 11;
      public const int EquipDye = 12;
      public const int HotbarItem = 13;
      public const int ChatItem = 14;
      public const int ShopItem = 15;
      public const int EquipGrapple = 16;
      public const int EquipMount = 17;
      public const int EquipMinecart = 18;
      public const int EquipPet = 19;
      public const int EquipLight = 20;
      public const int MouseItem = 21;
      public const int CraftingMaterial = 22;
      public const int Count = 23;
    }
  }
}
