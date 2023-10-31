// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Users.Drawables;
using osuTK;

namespace osu.Game.Users
{
    public partial class UserSquareList : VisibilityContainer
    {
        private readonly UsersList usersList = new UsersList();
        public bool IsTargetHovered
        {
            get => isTargetHovered;
            set
            {
                isTargetHovered = value;

                if (!isTargetHovered)
                {
                    schedulePopOut();
                }
            }
        }
        public List<APIUser>? Users
        {
            get => users;
            set
            {
                users = value;
                usersList.UpdateUsers(users ?? new List<APIUser>());
            }
        }
        protected override bool OnHover(HoverEvent e)
        {
            schedulePopOut();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            schedulePopOut();
            base.OnHoverLost(e);
        }

        public override void Show()
        {
            if (State.Value == Visibility.Visible)
            {
                schedulePopOut();
            }

            base.Show();
        }

        protected override void PopIn()
        {
            ClearTransforms();
            this.FadeIn(100);

            schedulePopOut();
        }

        private ScheduledDelegate? popOutDelegate;
        private List<APIUser>? users;
        private bool isTargetHovered;

        protected override void PopOut()
        {
            this.FadeOut(100);
        }

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            // keep the scheduled event correctly timed as long as we have movement.
            schedulePopOut();
            return base.OnMouseMove(e);
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AutoSizeAxes = Axes.Both;
            AutoSizeDuration = 200;
            AutoSizeEasing = Easing.OutQuint;
            Masking = true;
            CornerRadius = 5;

            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.9f,
                    Colour = colours.Gray3,
                },
                usersList
            });
        }

        protected override bool OnClick(ClickEvent e)
        {
            Console.WriteLine("Container clicked");
            return base.OnClick(e);
        }

        private void schedulePopOut()
        {
            popOutDelegate?.Cancel();
            this.Delay(1000).Schedule(() =>
            {
                if (!IsHovered && !IsTargetHovered)
                {
                    Hide();
                }
            }, out popOutDelegate);
        }

        private partial class UsersList : FillFlowContainer
        {
            public void UpdateUsers(List<APIUser> users)
            {
                var avatars = users.Select(u => new UpdateableAvatar(u).With(avatar =>
                {
                    avatar.Anchor = Anchor.TopLeft;
                    avatar.Origin = Anchor.TopLeft;
                    avatar.Size = new Vector2(30);
                    avatar.CornerRadius = 5;
                    avatar.Masking = true;
                })).Take(50).ToArray();
                Children = avatars;
                Width = users.Count <= 10 ? users.Count * 33 : 323;
            }

            public UsersList()
            {
                Anchor = Anchor.TopLeft;
                Origin = Anchor.TopLeft;
                Width = 323;
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Full;
                Spacing = new Vector2(2, 2);
                Padding = new MarginPadding(2);
            }
        }
    }
}
