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
        private UsersList usersList = new UsersList();
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
            Width = 400;
            Height = 200;
            AutoSizeDuration = 200;
            AutoSizeEasing = Easing.OutQuint;
            Masking = true;
            CornerRadius = 5;

            AddRange(new Drawable[]
            {
                new Box
                {
                    Width = 400,
                    Height = 200,
                    Alpha = 0.9f,
                    Colour = colours.Gray3,
                },
                usersList,
                // new FillFlowContainer()
                // {
                //     Margin = new MarginPadding(5),
                //     Spacing = new Vector2(10),
                //     AutoSizeAxes = Axes.Both,
                //     Direction = FillDirection.Vertical
                // }
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
                if (!IsHovered)
                    Hide();
            }, out popOutDelegate);
        }

        private partial class UsersList : FillFlowContainer
        {
            public void UpdateUsers(List<APIUser> users)
            {
                var avatars = users.Select(u => new UpdateableAvatar(u).With(avatar =>
                {
                    avatar.Anchor = Anchor.CentreLeft;
                    avatar.Origin = Anchor.CentreLeft;
                    avatar.Size = new Vector2(40);
                })).ToArray();
                Children = avatars;
            }

            public UsersList()
            {
                Anchor = Anchor.CentreLeft;
                Origin = Anchor.CentreLeft;
                AutoSizeAxes = Axes.Both;
                Direction = FillDirection.Horizontal;
                Spacing = new Vector2(10, 0);
            }
        }
    }
}
