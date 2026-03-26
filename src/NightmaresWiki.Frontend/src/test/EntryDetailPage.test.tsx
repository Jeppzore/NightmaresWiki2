import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import { EntryDetailPage } from "../pages/EntryDetailPage";

describe("EntryDetailPage", () => {
  it("renders stats and sections", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => ({
          type: "item",
          slug: "bronze-sword",
          title: "Bronze Sword",
          summary: "Starter weapon",
          taxonomy: ["Items", "Weapons", "Melee"],
          bodySections: [{ heading: "Trade & Crafting", content: "Crafted by Smithsson", items: [] }],
          stats: [{ label: "Damage", value: "1-2" }],
          relationships: [{ type: "droppedBy", title: "Green Slime", slug: "green-slime" }],
          sourcePath: "legacy",
          isDetailPage: true
        })
      }),
    );

    render(
      <MemoryRouter initialEntries={["/items/bronze-sword"]}>
        <Routes>
          <Route path="/items/:slug" element={<EntryDetailPage fallbackType="item" />} />
        </Routes>
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByRole("heading", { name: "Bronze Sword" })).toBeInTheDocument());
    expect(screen.getByText("Damage")).toBeInTheDocument();
    expect(screen.getByText("Crafted by Smithsson")).toBeInTheDocument();
  });
});
