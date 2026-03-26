import { NavLink, Outlet } from "react-router-dom";
import { SearchBox } from "./SearchBox";

const navigation = [
  { to: "/", label: "Home", end: true },
  { to: "/enemies", label: "Enemies" },
  { to: "/items", label: "Items" },
  { to: "/npcs", label: "NPC" },
];

export function Layout() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar__brand">
          <p className="sidebar__eyebrow">Nightmares</p>
          <h1>Wiki</h1>
          <p className="sidebar__copy">
            A Tibia-inspired archive for the world, loot, and creatures of Nightmares.
          </p>
        </div>

        <nav className="sidebar__nav" aria-label="Primary">
          {navigation.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.end}
              className={({ isActive }) => (isActive ? "nav-link nav-link--active" : "nav-link")}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="sidebar__note">
          <h2>Source</h2>
          <p>Imported from the legacy Nightmares wiki repository, then reshaped into a cleaner article experience.</p>
        </div>
      </aside>

      <div className="main-frame">
        <header className="topbar">
          <div>
            <p className="topbar__eyebrow">NightmaresWiki2</p>
            <h2 className="topbar__title">Fantasy field guide</h2>
          </div>
          <SearchBox />
        </header>

        <main className="page-content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
