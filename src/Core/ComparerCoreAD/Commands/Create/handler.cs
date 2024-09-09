﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Banhcafe.Microservices.ComparerCoreVsAD.Core.Common.Contracts.Response;
using Banhcafe.Microservices.ComparerCoreVsAD.Core.Common.Exceptions;
using Banhcafe.Microservices.ComparerCoreVsAD.Core.Common.Ports;
using Banhcafe.Microservices.ComparerCoreVsAD.Core.ComparerCoreAD.Models;
using Banhcafe.Microservices.ComparerCoreVsAD.Core.ComparerCoreAD.Ports;
using FluentValidation;
using MediatR;

namespace Banhcafe.Microservices.ComparerCoreVsAD.Core.ComparerCoreAD.Commands.Create
{
    public sealed class PopulateDataDigitalArchiveComparerCommand
        : IRequest<ApiResponse<DigitalArchiveComparerBase>>
    {
        public PopulateDataDigitalArchive data { get; set; }
    }

    public sealed class DigitalArchiveComparerCommandHandler(
        IDigitalArchiveComparerRepository repository,
        IUserContext userContext,
        IMapper mapper,
        IValidator<PopulateDataDigitalArchive> validator
    )
        : IRequestHandler<
            PopulateDataDigitalArchiveComparerCommand,
            ApiResponse<DigitalArchiveComparerBase>
        >
    {
        public async Task<ApiResponse<DigitalArchiveComparerBase>> Handle(
            PopulateDataDigitalArchiveComparerCommand request,
            CancellationToken cancellationToken
        )
        {
            var handleResponse = new ApiResponse<DigitalArchiveComparerBase>();

            var validationResults = await validator.ValidateAsync(request.data, cancellationToken);

            if (!validationResults.IsValid)
            {
                handleResponse.ValidationErrors = validationResults.ToDictionary();
                return handleResponse;
            }

            var dto = mapper.Map<PopulateDataDigitalArchiveComparerDto>(request.data);

            try
            {
                var results = await repository.Populate(dto, cancellationToken);
                handleResponse.Data = results;
            }
            catch (ServiceException ex)
            {
                handleResponse.AddError(ex.Message);
            }

            return handleResponse;
        }
    }
}