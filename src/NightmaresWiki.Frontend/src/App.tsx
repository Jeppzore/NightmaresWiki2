import { Navigate, Route, Routes } from "react-router-dom";
import { Layout } from "./components/Layout";
import { EntryDetailPage } from "./pages/EntryDetailPage";
import { EntryListPage } from "./pages/EntryListPage";
import { HomePage } from "./pages/HomePage";
import { NpcPage } from "./pages/NpcPage";
import { NotFoundPage } from "./pages/NotFoundPage";

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/enemies" element={<EntryListPage type="enemy" title="Enemies" />} />
        <Route path="/enemies/:slug" element={<EntryDetailPage fallbackType="enemy" />} />
        <Route path="/items" element={<EntryListPage type="item" title="Items" />} />
        <Route path="/items/:slug" element={<EntryDetailPage fallbackType="item" />} />
        <Route path="/npcs" element={<NpcPage />} />
        <Route path="/creatures" element={<Navigate to="/enemies" replace />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}
