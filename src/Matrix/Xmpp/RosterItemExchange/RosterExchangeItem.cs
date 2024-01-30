/*
 * Copyright (c) 2003-2020 by AG-Software <info@ag-software.de>
 *
 * All Rights Reserved.
 * See the COPYING file for more information.
 *
 * This file is part of the MatriX project.
 *
 * NOTICE: All information contained herein is, and remains the property
 * of AG-Software and its suppliers, if any.
 * The intellectual and technical concepts contained herein are proprietary
 * to AG-Software and its suppliers and may be covered by German and Foreign Patents,
 * patents in process, and are protected by trade secret or copyright law.
 *
 * Dissemination of this information or reproduction of this material
 * is strictly forbidden unless prior written permission is obtained
 * from AG-Software.
 *
 * Contact information for AG-Software is available at http://www.ag-software.de
 */

using Matrix.Attributes;

namespace Matrix.Xmpp.RosterItemExchange
{
    [XmppTag(Name = Tag.Item, Namespace = Namespaces.XRosterX)]
    public class RosterExchangeItem : Base.RosterItem
    {
        public RosterExchangeItem() : base(Namespaces.XRosterX)
        {
        }

        public Action Action
        {
            get
            {
                return !HasAttribute("action") ? Action.Add : GetAttributeEnum<Action>("action");
            }
            set { SetAttribute("action", value.ToString()); }
        }
    }
}