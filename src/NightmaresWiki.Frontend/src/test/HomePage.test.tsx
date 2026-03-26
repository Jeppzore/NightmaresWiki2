import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import { HomePage } from "../pages/HomePage";

describe("HomePage", () => {
  it("renders imported sections and highlights", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => ({
          title: "Nightmares Wiki",
          intro: "Imported intro",
          sections: [
            { type: "enemy", title: "Enemies", route: "/enemies", description: "Enemy archive", count: 3 },
            { type: "item", title: "Items", route: "/items", description: "Item archive", count: 32 },
            { type: "npc", title: "NPC", route: "/npcs", description: "Sparse NPC section", count: 0, statusNote: "No NPC detail pages exist in the legacy source yet." }
          ],
          highlights: [
            {
              type: "enemy",
              slug: "green-slime",
              title: "Green Slime",
              summary: "A green slime.",
              taxonomy: ["Enemies"],
              isDetailPage: true
            }
          ]
        })
      }),
    );

    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByText("Nightmares Wiki")).toBeInTheDocument());
    expect(screen.getByText("Enemy archive")).toBeInTheDocument();
    expect(screen.getByText("Green Slime")).toBeInTheDocument();
  });
});
