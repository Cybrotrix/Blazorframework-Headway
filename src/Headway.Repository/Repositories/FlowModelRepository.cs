﻿using Headway.Core.Interface;
using Headway.Core.Model;
using Headway.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headway.Repository.Repositories
{
    public class FlowRepository : RepositoryBase<FlowRepository>, IFlowRepository
    {
        public FlowRepository(ApplicationDbContext applicationDbContext, ILogger<FlowRepository> logger)
            : base(applicationDbContext, logger)
        {
        }

        public async Task<IEnumerable<Flow>> GetFlowsAsync()
        {
            return await applicationDbContext.Flows
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Flow> GetFlowAsync(int flowId)
        {
            var flow = await applicationDbContext.Flows
                .AsNoTracking()
                .Include(f => f.States)
                .FirstAsync(m => m.FlowId.Equals(flowId))
                .ConfigureAwait(false);

            return flow;
        }

        public async Task<Flow> AddFlowAsync(Flow flow)
        {
            await applicationDbContext.Flows
                .AddAsync(flow)
                .ConfigureAwait(false);

            await applicationDbContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return flow;
        }

        public async Task<Flow> UpdateFlowAsync(Flow flow)
        {
            var existing = await applicationDbContext.Flows
                .Include(f => f.States)
                .FirstOrDefaultAsync(m => m.FlowId.Equals(flow.FlowId))
                .ConfigureAwait(false);

            if (existing == null)
            {
                throw new NullReferenceException(
                    $"{nameof(flow)} FlowId {flow.FlowId} not found.");
            }
            else
            {
                applicationDbContext
                    .Entry(existing)
                    .CurrentValues.SetValues(flow);
            }

            var removeStates = (from state in existing.States
                                     where !flow.States.Any(s => s.StateId.Equals(state.StateId))
                                     select state)
                                     .ToList();

            applicationDbContext.RemoveRange(removeStates);

            foreach (var state in flow.States)
            {
                State existingState = null;

                if (state.StateId > 0)
                {
                    existingState = existing.States
                        .FirstOrDefault(s => s.StateId.Equals(state.StateId));
                }

                if (existingState == null)
                {
                    existing.States.Add(state);
                }
                else
                {
                    applicationDbContext.Entry(existingState).CurrentValues.SetValues(state);
                }
            }

            await applicationDbContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return flow;
        }

        public async Task<int> DeleteFlowAsync(int flowId)
        {
            var flow = await applicationDbContext.Flows
                .Include(f => f.States)
                .FirstOrDefaultAsync(f => f.FlowId.Equals(flowId))
                .ConfigureAwait(false);

            applicationDbContext.Remove(flow);

            return await applicationDbContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }
    }
}