// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osu.Game.Graphics.Sprites;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Overlays;
using osu.Game.Users.Drawables;
using osuTK;

namespace osu.Game.Users
{
    public partial class UserRecentFavouritedListPanel : VisibilityContainer
    {
        #region Fields

        private ScheduledDelegate? popOutDelegate;
        private FillFlowContainer? usersList;

        #endregion

        #region Public members

        public APIUser[] Users { set => setUsers(value); }

        #endregion

        #region Protected overrides

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

        protected override void PopIn()
        {
            ClearTransforms();
            this.FadeIn(100);

            schedulePopOut();
        }

        protected override void PopOut() => this.FadeOut(100);

        protected override bool OnMouseMove(MouseMoveEvent e)
        {
            // keep the scheduled event correctly timed as long as we have movement.
            schedulePopOut();
            return base.OnMouseMove(e);
        }

        #endregion

        public override void Show()
        {
            if (State.Value == Visibility.Visible)
            {
                schedulePopOut();
            }

            base.Show();
        }

        #region Private members

        [BackgroundDependencyLoader]
        private void load(OverlayColourProvider colourProvider)
        {
            AutoSizeAxes = Axes.Both;
            AutoSizeDuration = 200;
            AutoSizeEasing = Easing.OutQuint;
            Anchor = Anchor.Centre;
            Masking = true;
            CornerRadius = 5;

            AddRange(new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.9f,
                    Colour = colourProvider.Background6,
                },
                usersList = new FillFlowContainer
                {
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    AutoSizeAxes = Axes.Y,
                    Width = 300,
                    Direction = FillDirection.Full,
                    Spacing = new Vector2(2, 2),
                    Padding = new MarginPadding(2)
                    {
                        Left = 4,
                        Right = 4
                    }
                }
            });
        }

        private void schedulePopOut()
        {
            popOutDelegate?.Cancel();
            this.Delay(1000).Schedule(() =>
            {
                if (!IsHovered)
                {
                    Hide();
                }
            }, out popOutDelegate);
        }

        private void setUsers(APIUser[] users)
        {
            var avatars = users.Select(u => new ClickableAvatar(u, showCardOnHover: true).With(avatar =>
            {
                avatar.Anchor = Anchor.TopLeft;
                avatar.Origin = Anchor.TopLeft;
                avatar.Size = new Vector2(30);
                avatar.CornerRadius = 5;
                avatar.Masking = true;
            })).Take(50).ToArray();

            var contents = new List<Drawable>();
            contents.AddRange(avatars);

            if (users.Length > 50)
            {
                contents.Add(new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Child = new OsuSpriteText
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Margin = new MarginPadding
                        {
                            Right = 4
                        },
                        Text = $"+ {users.Length - 50}"
                    }
                });
            }

            if (usersList != null)
            {
                usersList.Children = contents;
            }
        }

        #endregion
    }
}
