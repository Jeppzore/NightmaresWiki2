import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { describe, expect, it, vi } from "vitest";
import { NpcPage } from "../pages/NpcPage";

describe("NpcPage", () => {
  it("renders the sparse NPC state", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({
        ok: true,
        json: async () => [
          {
            type: "npc",
            slug: "npcs",
            title: "NPC",
            summary: "Sparse NPC metadata",
            taxonomy: ["NPC"],
            isDetailPage: false
          }
        ]
      }),
    );

    render(
      <MemoryRouter>
        <NpcPage />
      </MemoryRouter>,
    );

    await waitFor(() => expect(screen.getByText("Sparse NPC metadata")).toBeInTheDocument());
    expect(screen.getByText("No dedicated NPC detail pages yet")).toBeInTheDocument();
  });
});
